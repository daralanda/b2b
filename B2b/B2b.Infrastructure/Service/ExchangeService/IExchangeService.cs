using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.ExchangeService
{
    public interface IExchangeService
    {
        ResultDto<Exchange> Add(Exchange data);
        ResultDto<Exchange> GetById(int id);
        ResultDto<Exchange> Remove(int id);
        ResultDto<Exchange> GetAll();
        ResultDto<Exchange> Update(Exchange data);
        ResultDto<Exchange> AutoUpdate();
    }
}
