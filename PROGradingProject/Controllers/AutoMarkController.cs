using BusinessLogic;
using Common.Models.Mark;
using Microsoft.AspNetCore.Mvc;
using PROGradingAPI.Controllers.Base;

namespace PROGradingAPI.Controllers
{
    public class AutoMarkController : BaseNewController
    {
        private readonly IAutoMarkService _autoMarkService;
        public AutoMarkController(IHttpContextAccessor httpContext, IConfiguration configuration, IAutoMarkService autoMarkService) : base(httpContext, configuration)
        {
            _autoMarkService = autoMarkService;
        }

        /// <summary>
        /// Auto mark
        /// </summary>
        /// <param name="autoMark"></param>
        /// <returns></returns>
        [HttpPost("Mark")]
        public IActionResult CreateAutoMark([FromBody] AutoMarkRequest autoMark)
        {
            var result = _autoMarkService.Mark(null, autoMark.StudentFolder, autoMark.TestCaseFolder, autoMark.ExamId, autoMark.StudentId);
            return StatusCode(200, result);
        }
    }
}
