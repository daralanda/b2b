using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.AccountService
{
    public interface IAccountService
    {
        ResultDto<AccountNumber> Add(AccountNumber data);
        ResultDto<AccountNumber> GetById(int id);
        ResultDto<AccountNumber> Remove(int id);
        ResultDto<AccountNumber> GetAll();
        ResultDto<AccountNumber> Update(AccountNumber data);
    }
}
