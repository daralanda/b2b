using B2b.Dal.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2b.Web.Middleware.JwtAuth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 1. [AllowAnonymous] kontrolü
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // 2. HTTP OPTIONS (CORS Preflight) isteklerini yetkilendirmeden muaf tutun!
        if (HttpMethods.IsOptions(context.HttpContext.Request.Method))
            return;

        // 3. Kullanıcı kontrolü
        var user = context.HttpContext.Items["LoginUser"] as User;
        if (user == null)
        {
            // JsonResult yerine standart UnauthorizedObjectResult kullanmak HTTP pipeline standartlarına daha uygundur
            context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized" });
        }
    }
}