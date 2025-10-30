using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.ResponseDto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace B2b.Infrastructure.Service.Authorization;

public class JwtUtils : IJwtUtils
{
    private readonly string _secretKey=string.Empty;
    private readonly B2bDbContext _context;
    public JwtUtils(string secretKey, B2bDbContext context)
    {
        _secretKey = secretKey;
        _context = context;
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim("id", user.UserId.ToString())]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public int? ValidateJwtToken(string token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            return userId;
        }
        catch
        {
            return null;
        }
    }
    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        AuthenticateResponse response = new();
        try
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == model.Email && x.Password == model.Password && x.IsActive == true && x.Role.IsActive == true);
            if (user == null) return new AuthenticateResponse();
            var token = GenerateJwtToken(user);
            response.Token = token;
            response.Email = user.Email;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.RoleId = user.RoleId;
            response.UserId = user.UserId;
            response.Password = user.Password;
        }
        catch (Exception ex)
        {
            string message = ex.Message;    
        }
        return response;

    }
}