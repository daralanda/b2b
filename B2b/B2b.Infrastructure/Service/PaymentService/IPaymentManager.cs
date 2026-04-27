
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.PaymentService
{
    public interface IPaymentManager
    {
        Task<PaymentTokenResponse> GetSSToken(int UserId);
        ResultDto<PaymentSetting> Add(PaymentSetting data);
        ResultDto<PaymentSetting> GetById(int id);
        ResultDto<PaymentSetting> Remove(int id);
        ResultDto<PaymentSetting> GetAll();
        ResultDto<PaymentSetting> Update(PaymentSetting data);
        Task<PaymentTokenResponse> GetManuelSSToken(int UserId, decimal Amount);
        ResultDto<PaymentHistory> ManuelPayment(Dictionary<string, string> data);
        ResultDto<PaymentHistory> GetAllHistory(int userId);


    }
}
