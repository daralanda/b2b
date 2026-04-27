namespace B2b.Infrastructure.ResponseDto
{
    public class PaymentOrderItem
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
