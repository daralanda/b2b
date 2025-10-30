namespace B2b.Infrastructure.ResponseDto
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public string UnitTypeName { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public int Vat { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int IsCampaign { get; set; }
        public decimal TotalDiscountRate { get; set; }
    }
}
