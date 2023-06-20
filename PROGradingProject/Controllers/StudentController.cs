using Common.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{

    [CustomAuthorize(Role.Student)]
    public class StudentController : BaseNewController
    {

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
