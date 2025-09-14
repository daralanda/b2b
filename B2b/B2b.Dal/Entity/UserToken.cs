using System.ComponentModel.DataAnnotations;
namespace B2b.Dal.Entity
{
    public class UserToken
    {
        [Key]
        public int UserTokenId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
