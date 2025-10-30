namespace B2b.Infrastructure.ResponseDto
{
    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        public string UnitTypeName { get; set; } = string.Empty;
        public int CurrencyId { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool IsDefault { get; set; }
    }
}
