using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UnitTypeId { get; set; }

    }
}
