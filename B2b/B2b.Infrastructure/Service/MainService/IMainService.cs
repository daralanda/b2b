using B2b.Dal.Entity;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.MainService
{
    public interface IMainService
    {
        User Login(AuthenticateRequest data);
        bool Register(User user);
        User GetUser(object user);
        SecurityObject GetDashboardUrl(string url);
        string GetDashboarMainMenu(string Domain, int? RoleId);
        string GetBreadcrumb(string SeoUrl, string Domain);
        bool IsPermission(string SeoUrl, string RunType, int roleId);
        string GetViewTitle(string url);
        User GetUserById(int UserId);
        SeoResponseDto GetSeo(string url);
    }
}
