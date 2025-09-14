using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public int Queue { get; set; }
        public virtual Product Product { get; set; }
    }
}
