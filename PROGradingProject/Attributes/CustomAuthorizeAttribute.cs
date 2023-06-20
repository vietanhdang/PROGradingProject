using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PROGradingAPI.Attributes;
using System.Net;
using System.Security.Claims;
using static Common.Enumeration.Enumeration;

namespace Common.Attributes
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(Role role) : base(typeof(CustomAuthorizeFilter))
        {
            Arguments = new object[] { role };
        }
    }
    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly Role _role;
        public CustomAuthorizeFilter(Role role)
        {
            _role = role;
        }
        private static IActionResult Unauthorized()
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            serviceResponse.OnError(message: "Unauthorized");
            serviceResponse.ErrorCode = (int)HttpStatusCode.Unauthorized;
            return new JsonResult(serviceResponse)
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            var userId = context.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = Unauthorized();
                return;
            }

            var userRole = context.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
            if (((int)_role).ToString() != userRole)
            {
                context.Result = Unauthorized();
                return;
            }
        }
    }
}
