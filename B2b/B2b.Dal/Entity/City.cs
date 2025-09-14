using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
}
