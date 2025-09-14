using System.ComponentModel.DataAnnotations;

namespace B2b.Infrastructure.RequestDto;
public class AuthenticateRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}