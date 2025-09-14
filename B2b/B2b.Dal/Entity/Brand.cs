using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ImageUrl { get; set; }
        public int Queno { get; set; }
    }
}
