using B2b.Dal.Entity;
using B2b.Infrastructure.Service.PaymentService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {
        private readonly IPaymentManager _paymentManager;

        public PaymentApiController(IPaymentManager paymentManager)
        {
            _paymentManager = paymentManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(await _paymentManager.GetSSToken(userId));
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_paymentManager.GetAll());
        }
        [HttpGet]
        public IActionResult GetById(int id)
        {
            return Ok(_paymentManager.GetById(id));
        }
        [HttpPost]
        public IActionResult Add(PaymentSetting data)
        {
            return Ok(_paymentManager.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_paymentManager.Remove(id));
        }
        [HttpPost]
        public IActionResult Update(PaymentSetting data)
        {
            return Ok(_paymentManager.Update(data));
        }

        [HttpGet]
        public IActionResult GetAllHistory()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(_paymentManager.GetAllHistory(userId));
        }
        [HttpPost]
        public async Task<IActionResult> GetManuelToken([FromBody]decimal amount)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(await _paymentManager.GetManuelSSToken(userId,amount));
        }
    }
}