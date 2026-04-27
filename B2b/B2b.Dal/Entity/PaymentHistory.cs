using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class PaymentHistory
    {
        [Key]
        public Guid Guid { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string SecuritySession { get; set; }
        public string Result { get; set; }
        public bool State { get; set; }
        public DateTime CreateDate { get; set; }
        public string MerchantPaymentId { get; set; }
    }
}
