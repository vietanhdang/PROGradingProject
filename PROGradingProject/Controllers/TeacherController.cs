using Common.Attributes;
using Microsoft.AspNetCore.Mvc;
using PROGradingAPI.Controllers.Base;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{
    [CustomAuthorize(Role.Teacher)]
    public class TeacherController : BaseNewController
    {
        public TeacherController(IHttpContextAccessor httpContext, IConfiguration configuration) : base(httpContext, configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
