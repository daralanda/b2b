using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.OrderService
{
    public interface IOrderService
    {
        ResultDto<Order> Add(Order data);
        ResultDto<Order> GetById(int id);
        ResultDto<Order> Remove(int id);
        ResultDto<Order> GetAll();
        ResultDto<Order> Update(Order data);
    }
}
