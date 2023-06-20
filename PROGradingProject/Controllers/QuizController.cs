using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PROGradingAPI.Controllers
{
    [Authorize]
    public class QuizController : BaseNewController
    {

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
