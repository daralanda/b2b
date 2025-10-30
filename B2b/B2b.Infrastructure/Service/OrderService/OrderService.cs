using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;


namespace B2b.Infrastructure.Service.OrderService
{
    public class OrderService(B2bDbContext context): IOrderService
    {
        private readonly B2bDbContext _context = context;
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
                                    join z in _context.ProductImages on x.ProductId equals z.ProductId into imgGroup
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
                FormattableString query = $"EXEC CreateOrderFromCart  @UserId={customerId}, @OrderNote='{dto.OrderNote}' , @PaymentType={dto.PaymentType}";
                var res = _context.Database
                                             .SqlQuery<CreateCartFromOrder>(query)
                                             .ToList().FirstOrDefault();
                data.ResultMessage = res.ResultMessage;
                data.OrderId = res.OrderId;
                data.OrderNo = res.OrderNo;
                data.FinalTotal = res.FinalTotal;
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

    }



}
