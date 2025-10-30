using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.CartService
{
    public interface ICartService
    {
        ResultDto<Cart> Add(Cart data);
        ResultDto<Cart> Remove(int id);
        ResultDto<CartResponseDto> GetAll(int UserId);
        ResultDto<Cart> Update(Cart data);
    }
}
