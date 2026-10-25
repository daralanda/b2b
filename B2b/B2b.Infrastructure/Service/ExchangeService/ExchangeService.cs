using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Infrastructure.Service.CurrencyService;
using B2b.Plugin.Exchange;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace B2b.Infrastructure.Service.ExchangeService
{
    public class ExchangeService(B2bDbContext context) : IExchangeService
    {
        // TCMB kurları günde bir kez yayınlanır; her istekte TCMB'ye gitmemek için uygulama genelinde kısa süreli önbellek
        private static readonly TimeSpan CentralBankCacheDuration = TimeSpan.FromMinutes(30);
        private static readonly object _centralBankLock = new();
        private static List<DailyExchangeDto> _centralBankCache;
        private static DateTime _centralBankCachedAt;

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
                    data.ExchangeId = res.ExchangeId;
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
                list = _context.Exchanges.OrderByDescending(p=>p.TransactionDate).ToList();
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
        // Her para biriminin en güncel kuru (footer'daki günlük kur paneli için)
        public ResultDto<DailyExchangeDto> GetDaily()
        {
            var daily = new List<DailyExchangeDto>();
            try
            {
                daily = (from c in _context.Currencies
                         let e = _context.Exchanges.Where(p => p.CurrencyId == c.CurrencyId)
                                                   .OrderByDescending(p => p.TransactionDate)
                                                   .FirstOrDefault()
                         where e != null
                         orderby c.CurrencyId
                         select new DailyExchangeDto
                         {
                             CurrencyCode = c.CurrencyCode,
                             CurrencyName = c.CurrencyName,
                             Buying = e.Buying,
                             Selling = e.Selling,
                             TransactionDate = e.TransactionDate,
                             Source = "Kayıtlı kur"
                         }).ToList();
                state = true;
                message = "Daily exchange retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<DailyExchangeDto>
            {
                State = state,
                Message = message,
                List = daily
            };
        }
        // Footer için USD/EUR kurları, doğrudan TCMB'den (30 dk önbellekli).
        // TCMB'ye ulaşılamazsa önce eski önbellek, o da yoksa veritabanındaki son kayıtlı kur döner.
        public ResultDto<DailyExchangeDto> GetCentralBank()
        {
            lock (_centralBankLock)
            {
                if (_centralBankCache != null && DateTime.Now - _centralBankCachedAt < CentralBankCacheDuration)
                    return new ResultDto<DailyExchangeDto> { State = true, Message = "Central bank exchange retrieved from cache.", List = _centralBankCache };

                var bulletin = ExchangePlugin.GetBulletin();
                if (bulletin != null && bulletin.CurrencyExchange.Count > 0)
                {
                    DateTime.TryParseExact(bulletin.Tarih, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bulletinDate);
                    _centralBankCache = bulletin.CurrencyExchange
                        .OrderBy(p => p.CurrencyCode == "USD" ? 0 : 1)
                        .Select(p => new DailyExchangeDto
                        {
                            CurrencyCode = p.CurrencyCode,
                            CurrencyName = p.Isim,
                            Buying = decimal.Parse(p.ForexBuying, CultureInfo.InvariantCulture),
                            Selling = decimal.Parse(p.ForexSelling, CultureInfo.InvariantCulture),
                            TransactionDate = bulletinDate,
                            Source = "TCMB"
                        }).ToList();
                    _centralBankCachedAt = DateTime.Now;
                    return new ResultDto<DailyExchangeDto> { State = true, Message = "Central bank exchange retrieved successfully.", List = _centralBankCache };
                }

                if (_centralBankCache != null)
                    return new ResultDto<DailyExchangeDto> { State = true, Message = "Central bank unreachable; returning last cached exchange.", List = _centralBankCache };
            }
            return GetDaily();
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
