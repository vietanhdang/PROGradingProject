using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PROGradingAPI.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseNewController : ControllerBase
    {
        public readonly IHttpContextAccessor _httpContext;
        public readonly IConfiguration _configuration;


        public BaseNewController(IHttpContextAccessor httpContext, IConfiguration configuration)
        {
            _httpContext = httpContext;
            _configuration = configuration;
        }
    }
}
