using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace PROGradingAPI.Attributes
{
    public class ViewInfoAuthorizationHandler : AuthorizationHandler<Requirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewInfoAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
        {
            var userId = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            var requestedUserId = _httpContextAccessor?.HttpContext?.Request.RouteValues["userId"]?.ToString();

            if (!string.IsNullOrEmpty(requestedUserId) && userId != requestedUserId)
            {
                context.Fail();
                ServiceResponse serviceResponse = new ServiceResponse();
                serviceResponse.OnError(message: "Unauthorized");
                serviceResponse.ErrorCode = (int)HttpStatusCode.Unauthorized;

                var responseJson = JsonConvert.SerializeObject(serviceResponse);
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                _httpContextAccessor.HttpContext.Response.ContentType = "application/json";
                return _httpContextAccessor.HttpContext.Response.WriteAsync(responseJson);
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}
