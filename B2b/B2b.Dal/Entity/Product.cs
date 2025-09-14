using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int CurrencyId { get; set; }
        public int Vat { get; set; }
        public int StockQuantity { get; set; }
        public string ProductCode { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductPrice> ProductPrices { get; set; }
        public virtual Currency Currency { get; set; }


    }
}
