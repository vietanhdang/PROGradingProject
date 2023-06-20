using Common.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{
    [CustomAuthorize(Role.Teacher)]
    public class TeacherController : BaseNewController
    {

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
