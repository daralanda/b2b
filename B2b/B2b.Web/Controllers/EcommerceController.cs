using B2b.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace B2b.Web.Controllers
{
    public class EcommerceController(ILogger<EcommerceController> logger) : Controller
    {
        private readonly ILogger<EcommerceController> _logger = logger;

        #region b2b
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult CampaignProducts()
        {
            return View();
        }
        public IActionResult OrderTrack()
        {
            return View();
        }
        public IActionResult MyCart()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult MyProfile()
        {
            return View();
        }
        public IActionResult AccountNumbers()
        {
            return View();
        }
        #endregion
    }
}
