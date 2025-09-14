using B2b.Dal.Entity;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.Authorization
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public int? ValidateJwtToken(string token);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
    }
}
