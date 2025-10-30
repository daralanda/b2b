using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Infrastructure.Service.CurrencyService;
using B2b.Plugin.Exchange;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.ExchangeService
{
    public class ExchangeService(B2bDbContext context) : IExchangeService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Exchange> list = new();
        Exchange row = new();
        public ResultDto<Exchange> Add(Exchange data)
        {
            try
            {
                var res=_context.Exchanges.Where(p=>p.CurrencyId == data.CurrencyId && p.TransactionDate.Date == DateTime.Now.Date).FirstOrDefault();
                if (res==null)
                {
                    data.TransactionDate = DateTime.Now;
                    _context.Exchanges.Add(data);
                    _context.SaveChanges();
                }
                else
                {
                    Update(data);
                }
                state = true;
                message = "Exchange added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Exchange>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Exchange> GetAll()
        {
            try
            {
                list = _context.Exchanges.ToList();
                state = true;
                message = "Exchange retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Exchange>
            {
                State = state,
                Message = message,
                List = list
            };
        }
        public ResultDto<Exchange> GetById(int id)
        {
            try
            {
                row = _context.Exchanges.Where(p => p.CurrencyId == id).FirstOrDefault();
                state = true;
                message = "Exchange retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Exchange>
            {
                State = state,
                Message = message,
                Data = row
            };
        }
        public ResultDto<Exchange> Remove(int id)
        {
            try
            {
                _context.Exchanges.RemoveRange(_context.Exchanges.Where(p => p.ExchangeId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "Exchange removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Exchange>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<Exchange> Update(Exchange data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Exchanges.Where(p => p.ExchangeId == data.ExchangeId).First();
                upd.Selling = data.Selling;
                upd.Buying = data.Buying;
                upd.CurrencyId = data.CurrencyId;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Exchange updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Exchange>
            {
                State = state,
                Message = message
            };
        }

        public ResultDto<Exchange> AutoUpdate()
        {
            try
            {
                var data = ExchangePlugin.GetExchanges();
                foreach (var item in data)
                {
                    Add(new Exchange
                    {
                        Buying = Convert.ToDecimal(item.ForexBuying.Replace(".",",")),
                        Selling = Convert.ToDecimal(item.ForexSelling.Replace(".", ",")),
                        CurrencyId = item.CurrencyCode == "USD" ? 2 : 3,
                        TransactionDate = DateTime.Now
                    });
                }
                state = true;
                message = "Exchange auto-updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Exchange>() { Message = message, State = state };
        }
    }
}
