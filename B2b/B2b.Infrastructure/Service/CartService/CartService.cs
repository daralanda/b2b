using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace B2b.Infrastructure.Service.CartService
{
    public class CartService(B2bDbContext context) : ICartService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Cart> carts = new();
        List<CartResponseDto> dto = new();
        public ResultDto<Cart> Add(Cart data)
        {
            try
            {
                var row = _context.Carts.Where(p => p.UserId == data.UserId && p.ProductId == data.ProductId).FirstOrDefault();
                FormattableString query = $"EXEC GetProductDiscountedPrice @ProductId={data.ProductId}, @CustomerId={data.UserId}";
                var price = _context.Database
                                             .SqlQuery<ProductDiscountPrice>(query)
                                             .ToList();
                data.Price = price.FirstOrDefault() != null ? price.FirstOrDefault().DiscountedPrice : 0;
                data.UnitTypeId = price.FirstOrDefault().UnitTypeId;
                if (row == null)
                {
                    _context.Carts.Add(data);
                    _context.SaveChanges();
                }
                else
                {
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                    row.Quantity += data.Quantity;
                    row.Price = data.Price;
                    _context.SaveChanges();
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
                state = true;
                message = "Cart added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Cart>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<CartResponseDto> GetAll(int UserId)
        {
            try
            {
                FormattableString query = $"EXEC GetCartDetails  @CustomerId={UserId}";
                dto = _context.Database
                                             .SqlQuery<CartResponseDto>(query)
                                             .ToList();
                state = true;
                message = "Cart retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<CartResponseDto>
            {
                State = state,
                Message = message,
                List = dto
            };
        }
        public ResultDto<Cart> Remove(int id)
        {
            try
            {
                var del = _context.Carts.Where(p => p.CartId == id).ToList();
                _context.Carts.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "Cart removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Cart>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<Cart> Update(Cart data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Carts.Where(p => p.CartId == data.CartId).First();
                upd.Price = data.Price;
                upd.Quantity = data.Quantity;
                upd.CreatedDate = DateTime.Now;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Cart updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Cart>
            {
                State = state,
                Message = message
            };
        }
    }
}
