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
        public IActionResult ProductDetails()
        {
            return View();
        }
        public IActionResult OrderTrack()
        {
            return View();
        }
        #endregion
    }
}
