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
        public int? CustomerId { get; set; }
        public Role Role { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<UserToken> UserTokens { get; set; }
    }
}
