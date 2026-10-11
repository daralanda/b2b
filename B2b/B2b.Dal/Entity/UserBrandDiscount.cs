using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2b.Dal.Entity
{
    public class UserBrandDiscount
    {
        [Key]
        public int UserBrandDiscountId { get; set; } // Alternatif olarak UserId ve BrandId birlikte Composite Key yapılabilir.
        public int UserId { get; set; }
        public int BrandId { get; set; }
        [Column(TypeName = "decimal(5,2)")] // Örn: 15.50 (%15.5 indirim) veya 100.00 (%100)
        public decimal DiscountRate { get; set; }
        public User User { get; set; } // Kullanıcı navigation property'si
        public Brand Brand { get; set; } // Marka navigation property'si

    }
}
