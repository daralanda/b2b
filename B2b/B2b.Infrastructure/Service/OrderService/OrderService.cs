using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Plugin.Mail;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Text;


namespace B2b.Infrastructure.Service.OrderService
{
    public class OrderService(B2bDbContext context, string companyEmail): IOrderService
    {
        private readonly B2bDbContext _context = context;
        private readonly string _companyEmail = companyEmail;
        bool state = false;
        string message = string.Empty;
        List<Order> orders = new();
        Order order = new();
        public ResultDto<Order> Add(Order data)
        {
            try
            {
                _context.Orders.Add(data);
                
                _context.SaveChanges();
                data.OrderDetails.ForEach(p => p.OrderId = data.OrderId);
                _context.OrderDetails.AddRange(data.OrderDetails);
                _context.SaveChanges();
                state = true;
                message = "Order added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Order> GetAll()
        {
            try
            {
                orders = _context.Orders.ToList();
                state = true;
                message = "Orders retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
                List = orders
            };
        }
        public ResultDto<OrderDto> GetById(int id)
        {
            var findOrder = new OrderDto();

            try
            {
                var data = _context.Orders.Where(p => p.OrderId == id).FirstOrDefault();
                var orderDetails = (from x in _context.OrderDetails
                                    join y in _context.Products on x.ProductId equals y.ProductId
                                    join z in _context.ProductImages.Where(i => i.ImageUrl != null && i.ImageUrl.Trim() != "") on x.ProductId equals z.ProductId into imgGroup
                                    from z in imgGroup.Take(1).DefaultIfEmpty()
                                    where x.OrderId == id
                                    select new OrderDetailDto
                                    {
                                        OrderId = x.OrderId,
                                        ProductId = x.ProductId,
                                        OrderDetailId = x.OrderDetailId,
                                        Count = x.Count,
                                        UnitType = x.UnitType,
                                        Discount = x.Discount,
                                        Price = x.Price,
                                        VatPrice = x.VatPrice,
                                        ProductName = y.ProductName,
                                        ProductImg = z.ImageUrl
                                    }).ToList();

                findOrder.OrderNote=data.OrderNote;
                findOrder.Total = data.Total;
                findOrder.OrderStatus = data.OrderStatus;
                findOrder.OrderDate = data.OrderDate;
                findOrder.OrderId = data.OrderId;
                findOrder.OrderNo = data.OrderNo;
                findOrder.OrderDetails = orderDetails;
                state = true;
                message = "Orders retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<OrderDto>
            {
                State = state,
                Message = message,
                Data = findOrder
            };
        }
        public ResultDto<Order> Remove(int id)
        {
            try
            {
                _context.Orders.RemoveRange(_context.Orders.Where(p => p.OrderId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "Order removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<Order> Update(Order data)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Orders.Where(p => p.OrderId == data.OrderId).First();
                upd.OrderStatus = data.OrderStatus;
                upd.OrderNote = data.OrderNote;
                upd.PaymentType = data.PaymentType;
                upd.Total = data.Total;
                upd.CustomerId = data.CustomerId;
                _context.SaveChanges();

                foreach (var item in data.OrderDetails)
                {
                    var rowUpt = _context.OrderDetails.Where(p => p.OrderDetailId == item.OrderDetailId).First();
                    rowUpt.VatPrice = item.VatPrice;
                    rowUpt.Price = item.Price;
                    rowUpt.Count = item.Count;
                    rowUpt.Discount = item.Discount;
                    _context.SaveChanges();
                }
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
   
                state = true;
                message = "Order updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message
            };
        }
        public ResultDto<CreateCartFromOrder> CreateOrder(RequestDto.OrderDto dto,int customerId)
        {
            var data = new CreateCartFromOrder();
            try
            {
                //Exec CreateOrderFromCart 1,'',2
                FormattableString query = $"EXEC CreateOrderFromCart  @UserId={customerId}, @OrderNote={dto.OrderNote}, @PaymentType={dto.PaymentType}";
                var res = _context.Database
                                             .SqlQuery<CreateCartFromOrder>(query)
                                             .ToList().FirstOrDefault();
                data.ResultMessage = res.ResultMessage;
                data.OrderId = res.OrderId;
                data.OrderNo = res.OrderNo;
                data.FinalTotal = res.FinalTotal;
                if (data.OrderId > 0)
                {
                    SendOrderMails(data.OrderId, customerId);
                }
                state = true;
                message = "Orders retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<CreateCartFromOrder>
            {
                State = state,
                Message = message,
                Data = data
            };
        }

        // Sipariş oluşunca firmaya (appsettings Mail:CompanyEmail) ve müşteriye bilgi maili gönderir.
        // SMTP ayarları MailSettings tablosundan okunur. Mail hatası siparişi etkilemez.
        private void SendOrderMails(int orderId, int customerId)
        {
            try
            {
                var mailConf = _context.MailSettings.FirstOrDefault();
                if (mailConf == null) return;

                var customer = _context.Users.Where(p => p.UserId == customerId).FirstOrDefault();
                var orderData = GetById(orderId).Data;
                if (orderData == null || orderData.OrderId == 0) return;

                var tr = new CultureInfo("tr-TR");
                string Enc(string s) => WebUtility.HtmlEncode(s ?? "");
                // IsIndividual true: kurumsal müşteri (firma adı), false/boş: bireysel müşteri (ad soyad)
                string customerName = customer == null ? "" :
                    customer.IsIndividual == true && !string.IsNullOrWhiteSpace(customer.CustomerName) ? customer.CustomerName : customer.FirstName + " " + customer.LastName;

                var rows = new StringBuilder();
                foreach (var item in orderData.OrderDetails)
                {
                    rows.Append("<tr>")
                        .Append("<td style='padding:6px;border:1px solid #ddd'>").Append(Enc(item.ProductName)).Append("</td>")
                        .Append("<td style='padding:6px;border:1px solid #ddd'>").Append(Enc(item.UnitType)).Append("</td>")
                        .Append("<td style='padding:6px;border:1px solid #ddd;text-align:right'>").Append(item.Count).Append("</td>")
                        .Append("<td style='padding:6px;border:1px solid #ddd;text-align:right'>").Append(item.Price.ToString("N2", tr)).Append("</td>")
                        .Append("<td style='padding:6px;border:1px solid #ddd;text-align:right'>").Append(item.Discount.ToString("N2", tr)).Append("</td>")
                        .Append("<td style='padding:6px;border:1px solid #ddd;text-align:right'>").Append(item.VatPrice.ToString("N2", tr)).Append("</td>")
                        .Append("</tr>");
                }

                string details =
                    "<p><b>Sipariş No:</b> " + Enc(orderData.OrderNo) + "<br/>" +
                    "<b>Sipariş Tarihi:</b> " + orderData.OrderDate.ToString("dd.MM.yyyy HH:mm", tr) + "<br/>" +
                    "<b>Müşteri:</b> " + Enc(customerName) + "<br/>" +
                    (customer == null ? "" : "<b>E-posta:</b> " + Enc(customer.Email) + "<br/><b>Telefon:</b> " + Enc(customer.Phone) + "<br/>") +
                    (string.IsNullOrWhiteSpace(orderData.OrderNote) ? "" : "<b>Sipariş Notu:</b> " + Enc(orderData.OrderNote) + "<br/>") +
                    "</p>" +
                    "<table style='border-collapse:collapse;font-size:13px'>" +
                    "<thead><tr style='background:#f3f3f3'>" +
                    "<th style='padding:6px;border:1px solid #ddd'>Ürün</th><th style='padding:6px;border:1px solid #ddd'>Birim</th>" +
                    // CreateOrderFromCart: Price = liste fiyatı, Discount = indirimli birim fiyat, VatPrice = satırın KDV tutarı
                    "<th style='padding:6px;border:1px solid #ddd'>Adet</th><th style='padding:6px;border:1px solid #ddd'>Liste Fiyatı</th>" +
                    "<th style='padding:6px;border:1px solid #ddd'>İndirimli Fiyat</th><th style='padding:6px;border:1px solid #ddd'>KDV Tutarı</th>" +
                    "</tr></thead><tbody>" + rows + "</tbody></table>" +
                    "<p><b>Toplam:</b> " + orderData.Total.ToString("N2", tr) + "</p>";

                string subject = "Sipariş No: " + orderData.OrderNo;
                string companyBody = "<p>Yeni bir sipariş alındı.</p>" + details;
                string customerBody = "<p>Sayın " + Enc(customerName) + ",</p><p>Siparişiniz başarıyla alınmıştır. Teşekkür ederiz.</p>" + details + "<p>Kare El Aletleri</p>";
                string companyEmail = _companyEmail;
                string customerEmail = customer?.Email;

                // SMTP gönderimi siparişi bekletmesin diye arka planda yapılır
                Task.Run(() =>
                {
                    if (!string.IsNullOrWhiteSpace(companyEmail))
                        MailPlugin.MailPost(mailConf.Host, mailConf.Port, mailConf.Email, mailConf.Password, mailConf.EnableSsl, companyEmail, "Yeni " + subject, companyBody);
                    if (!string.IsNullOrWhiteSpace(customerEmail))
                        MailPlugin.MailPost(mailConf.Host, mailConf.Port, mailConf.Email, mailConf.Password, mailConf.EnableSsl, customerEmail, "Siparişiniz Alındı - " + subject, customerBody);
                });
            }
            catch
            {
            }
        }

    }



}
