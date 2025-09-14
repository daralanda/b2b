using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class ProductPrice
    {
        [Key]
        public int ProductPriceId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int UnitTypeId { get; set; }
        public bool IsDefault { get; set; }
        public virtual Product Product { get; set; }
        public virtual UnitType UnitType { get; set; }
    }
}
