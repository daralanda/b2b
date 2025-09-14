using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.CurrencyService
{
    public interface ICurrencyService
    {
        ResultDto<Currency> Add(Currency data);
        ResultDto<Currency> GetById(int id);
        ResultDto<Currency> Remove(int id);
        ResultDto<Currency> GetAll();
        ResultDto<Currency> Update(Currency data);
    }
}
