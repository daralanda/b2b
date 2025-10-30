using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.ResponseDto;
using B2b.Plugin.Mail;

namespace B2b.Infrastructure.Service.MainService
{
    public class MainService : IMainService
    {
        private readonly B2bDbContext _dbContext;
        private readonly MailPlugin mailPlugin = new MailPlugin();
        public MainService(B2bDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public User Login(AuthenticateRequest data)
        {
            try
            {
                return _dbContext.Users.Where(p => p.IsActive == true &&
                p.Role.IsActive == true &&
                p.Email == data.Email &&
                p.Password == data.Password).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Register(User user)
        {
            try
            {
                user.IsActive = true;
                user.RoleId = _dbContext.Roles.Where(p => p.IsActive == true).First().RoleId;
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public User GetUser(object user)
        {
            if ((User)user != null)
            {
                return (User)user;
            }
            else
            {
                return null;
            }
        }
        public SecurityObject GetDashboardUrl(string url)
        {
            try
            {
                return _dbContext.SecurityObjects.Where(p => p.SeoUrl == url).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetDashboarMainMenu(string Domain, int? RoleId)
        {
            string strResult = string.Empty;
            try
            {
                var mainData = _dbContext.RolePermissions.Where(p => p.RoleId == RoleId && p.Selected && p.SecurityObject.IsMenu)
                 .Select(p => new
                 {
                     p.SecurityObject.Icon,
                     p.SecurityObjectId,
                     p.SecurityObject.Queno,
                     p.SecurityObject.ObjectDisplayName,
                     p.SecurityObject.SeoUrl,
                     p.SecurityObject.MainSecurityObjectId
                 }).OrderBy(p => p.Queno).ToList();
                var mainItems = mainData.Where(p => p.MainSecurityObjectId == 0).ToList();
                foreach (var item in mainItems)
                {
                    var subItems = mainData.Where(p => p.MainSecurityObjectId == item.SecurityObjectId).ToList();
                    if (subItems.Count > 0)
                    {
                        strResult += "<li class='nav-item dropdown'><a class='nav-link dropdown-toggle arrow-none'><i class='" + item.Icon + " me-2'></i><span>" + item.ObjectDisplayName + "</span> <div class='arrow-down'></div></a>" +
                                            "<div class='dropdown-menu' aria-expanded='false'>";
                        foreach (var subItem in subItems)
                        {
                            strResult += "<a class='dropdown-item' href='" + Domain + subItem.SeoUrl + "'>" + subItem.ObjectDisplayName + "</a>";
                        }
                        strResult += "</div></li>";

                    }
                    else
                    {
                        strResult += "<li class='nav-item dropdown' ><a href='" + Domain + item.SeoUrl + "' class='nav-link dropdown-toggle arrow-none'><i class='" + item.Icon + " me-2'></i><span>" + item.ObjectDisplayName + "</span></a></li>";
                    }
                }

            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }
            return strResult;
        }
        public string GetBreadcrumb(string SeoUrl, string Domain)
        {
            string strResult = string.Empty;
            var data = _dbContext.SecurityObjects.Where(p => p.SeoUrl == SeoUrl).FirstOrDefault();
            if (data != null)
            {
                strResult = "<h4 class='mb-0 font-size-18'>" + data.ObjectDisplayName + "</h4>" +
                             "<div class='page-title-right'>" +
                             "<ol class='breadcrumb m-0'>" +
                                  "<li class='breadcrumb-item'><a href='" + Domain + "/Dashboard" + "'>Anasayfa</a></li>";
                if (data.MainSecurityObjectId != 0)
                {
                    var mainData = _dbContext.SecurityObjects.Where(p => p.SecurityObjectId == data.MainSecurityObjectId).First();
                    strResult += "<li class='breadcrumb-item'><a href='#'>" + mainData.ObjectDisplayName + "</a></li>";
                }
                strResult += "<li class='breadcrumb-item active'>" + data.ObjectDisplayName + "</li>";
            }
            else
            {
                strResult = "<h4 class='mb-0 font-size-18'>Anasayfa</h4>" +
                           "<div class='page-title-right'>" +
                           "<ol class='breadcrumb m-0'>" +
                                "<li class='breadcrumb-item'><a href='" + Domain + "/Dashboard" + "'>Anasayfa</a></li>";
            }
            strResult += "</ol></div>";
            return strResult;
        }
        public bool IsPermission(string SeoUrl, string RunType, int roleId)
        {
            bool state = false;
            var data = _dbContext.RolePermissions.Where(p => p.RoleId == roleId && p.Role.IsActive && p.SecurityObject.SeoUrl == SeoUrl).FirstOrDefault();
            if (data != null)
            {
                switch (RunType)
                {
                    case "Insert":
                        state = data.Inserted;
                        break;
                    case "Select":
                        state = data.Selected;
                        break;
                    case "Update":
                        state = data.Updated;
                        break;
                    case "Delete":
                        state = data.Deleted;
                        break;
                }

            }
            return state;
        }
        public string GetViewTitle(string url)
        {
            try
            {
                var data = _dbContext.SecurityObjects.Where(p => p.SeoUrl == url).FirstOrDefault();
                if (data != null)
                {
                    return data.ObjectDisplayName;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        public User GetUserById(int UserId)
        {
            try
            {
                return _dbContext.Users.Where(p => p.UserId == UserId).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public SeoResponseDto GetSeo(string url)
        {
            var result = new SeoResponseDto()
            {
                ActionName = "Index",
                ControllerName = "Home",
                SeoUrl = "/",
            };
            url = string.IsNullOrEmpty(url) ? "/" : url;
            int id = 0;
            if (url.Contains("?id="))
            {

            }
            var data = _dbContext.SecurityObjects.Where(p => url == (p.SeoUrl)).FirstOrDefault();
            if (data != null)
            {
                result.SeoId = id;
                result.ControllerName = data.ControllerName;
                result.ActionName = data.ActionName;
                result.Title = data.ObjectDisplayName;
            }
            else
            {
                if (url == "/login")
                {
                    result.SeoId = id;
                    result.ControllerName = "Home";
                    result.ActionName = "Login";
                    result.Title = "Kullanıcı Girişi";
                }
                else if (url == "/register")
                {
                    result.SeoId = id;
                    result.ControllerName = "Home";
                    result.ActionName = "Register";
                    result.Title = "Kullanıcı Kayıt";
                }
                else if (url == "/recover-password")
                {
                    result.SeoId = id;
                    result.ControllerName = "Home";
                    result.ActionName = "Recoverpw";
                    result.Title = "Şifremi Unuttum";
                }
                else if (url == "/logout")
                {
                    result.SeoId = id;
                    result.ControllerName = "Home";
                    result.ActionName = "Login";
                    result.Title = "Güvenli Çıkış";
                }
                else if (url == "/profile")
                {
                    result.SeoId = id;
                    result.ControllerName = "Ecommerce";
                    result.ActionName = "MyProfile";
                    result.Title = "Kullanıcı Bilgilerim";
                }
                else if (url == "/sepetim")
                {
                    result.SeoId = id;
                    result.ControllerName = "Ecommerce";
                    result.ActionName = "MyCart";
                    result.Title = "Sepetim";
                }
                else if (url == "/odeme")
                {
                    result.SeoId = id;
                    result.ControllerName = "Ecommerce";
                    result.ActionName = "Checkout";
                    result.Title = "Sepetim";
                }
            }
            return result;
        }      
    }
}
