using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.CurrencyService
{
    public class CurrencyService(B2bDbContext context) : ICurrencyService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Currency> list = new();
        Currency row = new();
        public ResultDto<Currency> Add(Currency data)
        {
            try
            {
                _context.Currencies.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Currency added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Currency>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Currency> GetAll()
        {
            try
            {
                list = _context.Currencies.ToList();
                state = true;
                message = "Currencys retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Currency>
            {
                State = state,
                Message = message,
                List = list
            };
        }
        public ResultDto<Currency> GetById(int id)
        {
            try
            {
                row = _context.Currencies.Where(p => p.CurrencyId == id).FirstOrDefault();
                state = true;
                message = "Currencys retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Currency>
            {
                State = state,
                Message = message,
                Data = row
            };
        }
        public ResultDto<Currency> Remove(int id)
        {
            try
            {
                _context.Currencies.RemoveRange(_context.Currencies.Where(p => p.CurrencyId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "Currency removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Currency>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<Currency> Update(Currency data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Currencies.Where(p => p.CurrencyId == data.CurrencyId).First();
                upd.CurrencyName = data.CurrencyName;
                upd.CurrencyCode = data.CurrencyCode;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Currency updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Currency>
            {
                State = state,
                Message = message
            };
        }
    }
}
