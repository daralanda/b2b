namespace B2b.Plugin.Dto
{
    public class EPPlusDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string UnitTypeName { get; set; }
        public decimal Price { get; set; }
        public string CurrencyName { get; set; }
        public int Vat { get; set; }
        public string IsDefault { get; set; }
        public int StockQuantity { get; set; }

        public ResultDto Result { get; set; }
    }
    public class ResultDto
    {
        public bool State { get; set; }
        public string Message { get; set; }
    }
}
