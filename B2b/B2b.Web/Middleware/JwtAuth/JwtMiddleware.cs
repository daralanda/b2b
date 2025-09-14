using B2b.Infrastructure.Service.Authorization;
using B2b.Infrastructure.Service.UserService;

namespace B2b.Web.Middleware.JwtAuth;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtUtils jwtUtils,IUserService userService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateJwtToken(token);
        if (userId != null)
        {
           var data= userService.GetById(userId.Value);
           context.Items["LoginUser"] =data;
        }
       await _next(context);
    }
}