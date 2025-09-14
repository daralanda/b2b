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
        public double Discount { get; set; }
        public double Price { get; set; }
    }
}
