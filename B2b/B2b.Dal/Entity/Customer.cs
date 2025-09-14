using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNo { get; set; }
        public string Address { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsActive { get; set; }
    }
}
