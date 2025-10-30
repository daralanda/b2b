using B2b.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace B2b.Web.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            HttpContext.Items.Clear();
            HttpContext.Session.Clear();
            return View();
        }

        #region Tanýmlamalar

        public IActionResult CampaignList()
        {
            return View();
        }
        //ürün
        public IActionResult AccountNumbers()
        {
            return View();
        }
        public IActionResult BannerList()
        {
            return View();
        }
        public IActionResult BrandList()
        {
            return View();
        }
        public IActionResult CategoryList()
        {
            return View();
        }
        public IActionResult OrderList()
        {
            return View();
        }
        public IActionResult ProductList()
        {
            return View();
        }
        public IActionResult UserList()
        {
            return View();
        }
        public IActionResult RoleList()
        {
            return View();
        }
        public IActionResult SliderList()
        {
            return View();
        }
        public IActionResult CurrencyList()
        {
            return View();
        }
        public IActionResult ExchangeList()
        {
            return View();
        }
        #endregion

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Recoverpw()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
