namespace B2b.Infrastructure.ResponseDto
{
    public class DailyExchangeDto
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public decimal Buying { get; set; }
        public decimal Selling { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Source { get; set; }
    }
}
