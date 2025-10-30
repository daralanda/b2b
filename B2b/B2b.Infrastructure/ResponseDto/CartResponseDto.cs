namespace B2b.Infrastructure.ResponseDto
{
    public class CartResponseDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Quantity { get; set; }
        public int Vat { get; set; }
        public string UnitTypeName { get; set; }
        public decimal VatPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
