using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal VatPrice { get; set; }
        public string UnitType { get; set; }
        public virtual Order Order { get; set; }
    }
}
