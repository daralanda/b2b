using System.ComponentModel.DataAnnotations;
namespace B2b.Dal.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string CustomerName { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNo { get; set; }
        public string Address { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public bool? IsIndividual { get; set; }
        public int? DiscountRate { get; set; }
        public Role Role { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<UserToken> UserTokens { get; set; }
    }
}
