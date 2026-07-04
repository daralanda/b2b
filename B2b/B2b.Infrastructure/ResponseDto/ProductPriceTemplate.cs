namespace B2b.Infrastructure.ResponseDto
{
    public class ProductPriceTemplate
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string UnitTypeName { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
