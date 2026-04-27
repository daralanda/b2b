using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Text.Json;

namespace B2b.Infrastructure.Service.PaymentService
{
    public class PaymentManager : IPaymentManager
    {
        private readonly B2bDbContext _context;
        bool state = false;
        string message = string.Empty;
        List<PaymentSetting> list = new();
        PaymentSetting row = new();
        public PaymentManager(B2bDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTokenResponse> GetSSToken(int UserId) 
        {
            var result = new PaymentTokenResponse();
            FormattableString data = $"EXEC sp_GetPaymentSessionDetails @UserId={UserId}";
            var resultdata = _context.Database.SqlQuery<SecuritySessionToken>(data).ToList().FirstOrDefault();
            using var _httpClient = new HttpClient();
            // 1. Verileri Form formatına hazırlıyoruz
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ACTION", resultdata.ACTION),
                new KeyValuePair<string, string>("AMOUNT", resultdata.AMOUNT.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("CURRENCY", resultdata.CURRENCY),
                new KeyValuePair<string, string>("CUSTOMER", resultdata.CUSTOMER),
                new KeyValuePair<string, string>("CUSTOMERNAME", resultdata.CUSTOMERNAME),
                new KeyValuePair<string, string>("CUSTOMEREMAIL", resultdata.CUSTOMEREMAIL),
                new KeyValuePair<string, string>("CUSTOMERPHONE", resultdata.CUSTOMERPHONE),
                new KeyValuePair<string, string>("MERCHANT", resultdata.MERCHANT),
                new KeyValuePair<string, string>("MERCHANTUSER", resultdata.MERCHANTUser),
                new KeyValuePair<string, string>("MERCHANTPASSWORD", resultdata.MERCHANTUserPass),
                new KeyValuePair<string, string>("MERCHANTPAYMENTID", resultdata.MERCHANTPAYMENTID),
                new KeyValuePair<string, string>("RETURNURL", resultdata.RETURNURL),
                new KeyValuePair<string, string>("SESSIONTYPE", resultdata.SESSIONTYPE),
                new KeyValuePair<string, string>("ORDERITEMS", resultdata.OrderItems)
            };
            var content = new FormUrlEncodedContent(formData);
            string baseUrl = _context.PaymentSettings.Where(p => p.IsDefault == true).Select(p => p.BaseUrl).FirstOrDefault();
            var response = await _httpClient.PostAsync(baseUrl, content);
            if (response.IsSuccessStatusCode)
            {
                string json=await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<PaymentTokenResponse>(json, options);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        public ResultDto<PaymentSetting> Add(PaymentSetting data)
        {
            try
            {
                _context.PaymentSettings.Add(data);
                _context.SaveChanges();
                state = true;
                message = "PaymentSetting added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<PaymentSetting>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<PaymentSetting> GetAll()
        {
            try
            {
                list = _context.PaymentSettings.ToList();
                state = true;
                message = "PaymentSettings retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<PaymentSetting>
            {
                State = state,
                Message = message,
                List = list
            };
        }
        public ResultDto<PaymentSetting> GetById(int id)
        {
            try
            {
                row = _context.PaymentSettings.Where(p => p.PaymentSettingId == id).FirstOrDefault();
                state = true;
                message = "PaymentSetting retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<PaymentSetting>
            {
                State = state,
                Message = message,
                Data = row
            };
        }
        public ResultDto<PaymentSetting> Remove(int id)
        {
            try
            {
                _context.PaymentSettings.RemoveRange(_context.PaymentSettings.Where(p => p.PaymentSettingId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "PaymentSetting removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<PaymentSetting>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<PaymentSetting> Update(PaymentSetting data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.PaymentSettings.Where(p => p.PaymentSettingId == data.PaymentSettingId).First();
                upd.Merchant = data.Merchant;
                upd.MerchantUser = data.MerchantUser;
                upd.MerchantPassword = data.MerchantPassword;
                upd.BaseUrl = data.BaseUrl;
                upd.ReturnUrl = data.ReturnUrl;
                upd.Currency = data.Currency;
                upd.SessionType = data.SessionType;
                upd.IsDefault = data.IsDefault;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "PaymentSettign updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<PaymentSetting>
            {
                State = state,
                Message = message
            };
        }



        public async Task<PaymentTokenResponse> GetManuelSSToken(int UserId,decimal Amount)
        {
            var result = new PaymentTokenResponse();
            FormattableString data = $"EXEC sp_GetPaymentSessionManuel @UserId={UserId} ,@Amount={Amount}";
            var resultdata = _context.Database.SqlQuery<SecuritySessionToken>(data).ToList().FirstOrDefault();
            using var _httpClient = new HttpClient();
            // 1. Verileri Form formatına hazırlıyoruz
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ACTION", resultdata.ACTION),
                new KeyValuePair<string, string>("AMOUNT", resultdata.AMOUNT.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("CURRENCY", resultdata.CURRENCY),
                new KeyValuePair<string, string>("CUSTOMER", resultdata.CUSTOMER),
                new KeyValuePair<string, string>("CUSTOMERNAME", resultdata.CUSTOMERNAME),
                new KeyValuePair<string, string>("CUSTOMEREMAIL", resultdata.CUSTOMEREMAIL),
                new KeyValuePair<string, string>("CUSTOMERPHONE", resultdata.CUSTOMERPHONE),
                new KeyValuePair<string, string>("MERCHANT", resultdata.MERCHANT),
                new KeyValuePair<string, string>("MERCHANTUSER", resultdata.MERCHANTUser),
                new KeyValuePair<string, string>("MERCHANTPASSWORD", resultdata.MERCHANTUserPass),
                new KeyValuePair<string, string>("MERCHANTPAYMENTID", resultdata.MERCHANTPAYMENTID),
                new KeyValuePair<string, string>("RETURNURL", resultdata.RETURNURL),
                new KeyValuePair<string, string>("SESSIONTYPE", resultdata.SESSIONTYPE),
                new KeyValuePair<string, string>("ORDERITEMS", resultdata.OrderItems)
            };
            var content = new FormUrlEncodedContent(formData);
            string baseUrl = _context.PaymentSettings.Where(p => p.IsDefault == true).Select(p => p.BaseUrl).FirstOrDefault();
            var response = await _httpClient.PostAsync(baseUrl, content);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<PaymentTokenResponse>(json, options);
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var history = _context.PaymentHistories.Where(p => p.UserId == UserId && p.Amount == Amount).FirstOrDefault();
                history.SecuritySession = result.SessionToken;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        public ResultDto<PaymentHistory> ManuelPayment(Dictionary<string, string> data)
        {
            var resultDto=new ResultDto<PaymentHistory>();
            string paymentId = data.Where(p => p.Key == "merchantPaymentId").Select(p => p.Value).FirstOrDefault();
            string result = JsonSerializer.Serialize(data);
            bool state = data.Where(p => p.Key.ToLower() == "responsecode").Select(p => p.Value).FirstOrDefault() == "00" ? true : false;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            var history = _context.PaymentHistories.Where(p => p.MerchantPaymentId == paymentId).FirstOrDefault();
            history.State = state;
            history.Result = result;
            _context.SaveChanges();
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            resultDto.State = state;
            resultDto.Message = state ? data.Where(p => p.Key.ToLower() == "responsecode").Select(p => p.Value).FirstOrDefault() : "İşlem Başarılıdır.";

            return resultDto;
        }

        public ResultDto<PaymentHistory> GetAllHistory(int userId)
        {
            var result = new ResultDto<PaymentHistory>();
            try
            {
                result.List = _context.PaymentHistories.Where(p => p.UserId == userId).ToList();
                result.State = true;
                message = "Payment Histories retrieved successfully.";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                state = false;
            }
            return result;
        }
    }
}