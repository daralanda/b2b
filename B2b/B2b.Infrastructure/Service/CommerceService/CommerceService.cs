using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto; // ResultDto sınıfı burada
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
namespace B2b.Infrastructure.Service.CommerceService
{
    public class CommerceService(B2bDbContext context) : ICommerceService
    {
        private readonly B2bDbContext _context = context;
        public ResultDto<ProductDetailDto> GetProducts(int CustomerId)
        {
            var result = new ResultDto<ProductDetailDto>();

            try
            {
                FormattableString data= $"EXEC GetProductDetailsWithTLPrice @CustomerId={CustomerId}";
                var products =  _context.Database
                                             .SqlQuery<ProductDetailDto>(data)
                                             .ToList();
                result.State = true;
                result.List = products.ToList();
                result.Message = "Ürünler başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }
        public ResultDto<ProductDetailDto> GetProductInfo(int CustomerId,int ProductId)
        {
            var result = new ResultDto<ProductDetailDto>();

            try
            {
                FormattableString data = $"EXEC GetProductInfo @CustomerId={CustomerId},@ProductId={ProductId}";
                var products = _context.Database
                                             .SqlQuery<ProductDetailDto>(data)
                                             .ToList();
                result.State = true;
                result.Data = products.FirstOrDefault();
                result.Message = "Ürün başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }
        public ResultDto<Category> GetCategories()
        {
            var result = new ResultDto<Category>();

            try
            {
                result.State = true;
                result.List = _context.Categories.Where(p=>p.IsActive==true).OrderByDescending(p=>p.CategoryId).ToList();
                result.Message = "Kategori başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }
        public ResultDto<ProductDetailsDto> GetProduct(int ProductId,int CustomerId)
        {
            var result = new ResultDto<ProductDetailsDto>();

            try
            {
                FormattableString data = $"EXEC GetProductPricesByProductId @ProductId={ProductId},@CustomerId={CustomerId}";
                var list = _context.Database
                                             .SqlQuery<ProductDetailsDto>(data)
                                             .ToList();
                result.State = true;
                result.List = list.ToList();
                result.Message = "Ürün başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }
        public ResultDto<ProductImage> GetProductImage(int ProductId)
        {
            var result = new ResultDto<ProductImage>();

            try
            {
                result.State = true;
                result.List = _context.ProductImages.Where(p => p.ProductId == ProductId).OrderBy(p => p.Queue).ToList();
                result.Message = "Resimler başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }

        public ResultDto<ProductDetailDto> GetCampaignProducts(int CustomerId)
        {
            var result = new ResultDto<ProductDetailDto>();

            try
            {
                FormattableString data = $"EXEC GetCampaignProducts @CustomerId={CustomerId}";
                var products = _context.Database
                                             .SqlQuery<ProductDetailDto>(data)
                                             .ToList();
                result.State = true;
                result.List = products.ToList();
                result.Message = "Ürünler başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }
        public ResultDto<ProductDetailDto> GetCampaignProductList(int CustomerId)
        {
            var result = new ResultDto<ProductDetailDto>();

            try
            {
                FormattableString data = $"EXEC GetCampaignProductList @CustomerId={CustomerId}";
                var products = _context.Database
                                             .SqlQuery<ProductDetailDto>(data)
                                             .ToList();
                result.State = true;
                result.List = products.ToList();
                result.Message = "Ürünler başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }

        public ResultDto<ProductDetailDto> GetLatestProducts(int CustomerId)
        {
            var result = new ResultDto<ProductDetailDto>();

            try
            {
                FormattableString data = $"EXEC GetLatestProducts @CustomerId={CustomerId}";
                var products = _context.Database
                                             .SqlQuery<ProductDetailDto>(data)
                                             .ToList();
                result.State = true;
                result.List = products.ToList();
                result.Message = "Ürünler başarıyla getirildi.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = $"Bir hata oluştu: {ex.Message}";
            }

            return result;
        }

        public ResultDto<Order> GetAllOrder(int id)
        {
            bool state = false;
            string message = string.Empty;
            var orders = new List<Order>();
            try
            {
                orders= _context.Orders.Where(p=>p.CustomerId==id).ToList();
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
                List = orders.ToList()
            };
        }
        public ResultDto<OrderDto> GetByOrderId(int id)
        {
            var findOrder = new OrderDto();
           bool state= false;
            string message = string.Empty;
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

                findOrder.OrderNote = data.OrderNote;
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
    }

}