using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class District
    {
        [Key]
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int CityId { get; set; }
    }
}
