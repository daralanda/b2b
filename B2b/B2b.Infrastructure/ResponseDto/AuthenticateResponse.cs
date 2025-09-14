using B2b.Dal.Entity;

namespace B2b.Infrastructure.ResponseDto
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public virtual Role Role { get; set; }
        public List<UserToken> UserTokens { get; set; }

    }
}