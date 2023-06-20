using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PROGradingAPI.Controllers
{
    public class AutoMarkController : BaseNewController
    {

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
