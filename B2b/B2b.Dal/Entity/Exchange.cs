using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Exchange
    {
        [Key]
        public int ExchangeId { get; set; }
        public decimal Buying { get; set; }
        public decimal Selling { get; set; }
        public int CurrencyId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
