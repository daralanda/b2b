using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class PaymentSetting
    {
        [Key]
        public int PaymentSettingId { get; set; }
        public int Merchant { get; set; }
        public string MerchantUser { get; set; }
        public string MerchantPassword { get; set; }
        public string BaseUrl { get; set; }
        public string Currency { get; set; }
        public string ReturnUrl { get; set; }
        public string SessionType { get; set; }
        public bool IsDefault { get; set; }
    }
}
