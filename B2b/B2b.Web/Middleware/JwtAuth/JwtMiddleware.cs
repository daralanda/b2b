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

    public async Task Invoke(HttpContext context, IJwtUtils jwtUtils, IUserService userService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var userId = jwtUtils.ValidateJwtToken(token);

            if (userId != null)
            {
                try
                {
                    // EF Core ve DB işlemlerini asenkron çağırmak performans ve kilitlenmeler için kritiktir
                    var data = await userService.GetByIdAsync(userId.Value);
                    if (data != null)
                    {
                        context.Items["LoginUser"] = data;
                    }
                }
                catch
                {
                    // Token geçerli olsa bile DB tarafında bir sorun çıkarsa isteğin patlamasını engelliyoruz
                }
            }
        }

        await _next(context);
    }
}