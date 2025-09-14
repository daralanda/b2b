using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Banner
    {
        [Key]
        public int BannerId { get; set; }
        public string BannerName { get; set; }
        public string ImageUrl { get; set; }
        public string BannerUrl { get; set; }
        public bool IsActive { get; set; }
        public int BannerType { get; set; }
        public int Queno { get; set; }
    }
}
