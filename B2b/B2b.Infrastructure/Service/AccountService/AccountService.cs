using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.AccountService
{
    public class AccountService(B2bDbContext context) : IAccountService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<AccountNumber> accountNumbers = new();
        AccountNumber accountNumber = new();
        public ResultDto<AccountNumber> Add(AccountNumber data)
        {
            try
            {
                _context.AccountNumbers.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Account Number added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<AccountNumber>
            {
                State = state,
                Message = message,
                Data = data
            };
        }
        public ResultDto<AccountNumber> GetAll()
        {
            try
            {
                accountNumbers = _context.AccountNumbers.ToList();
                state = true;
                message = "Account Numbers retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<AccountNumber>
            {
                State = state,
                Message = message,
                List = accountNumbers
            };
        }
        public ResultDto<AccountNumber> GetById(int id)
        {
            try
            {
                accountNumber = _context.AccountNumbers.Where(p => p.AccountNumberId == id).FirstOrDefault();
                state = true;
                message = "Account Number retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<AccountNumber>
            {
                State = state,
                Message = message,
                Data = accountNumber
            };
        }
        public ResultDto<AccountNumber> Remove(int id)
        {
            try
            {
                var del = _context.AccountNumbers.Where(p => p.AccountNumberId == id).ToList();
                _context.AccountNumbers.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "AccountNumber removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<AccountNumber>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<AccountNumber> Update(AccountNumber data)
        {
            try
            {

                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.AccountNumbers.Where(p => p.AccountNumberId == data.AccountNumberId).First();
                upd.AccountName = data.AccountName;
                upd.AccountNo = data.AccountNo;
                upd.Bank = data.Bank;
                upd.Currency= data.Currency;
                upd.BranchCode = data.BranchCode;
                upd.IBAN = data.IBAN;
                upd.Logo = data.Logo;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "AccountNumber updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<AccountNumber>
            {
                State = state,
                Message = message
            };
        }
    }
}
