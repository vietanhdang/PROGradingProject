using Common.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROGradingAPI.Controllers.Base;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{

    [CustomAuthorize(Role.Student)]
    public class StudentController : BaseNewController
    {
        public StudentController(IHttpContextAccessor httpContext, IConfiguration configuration) : base(httpContext, configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
