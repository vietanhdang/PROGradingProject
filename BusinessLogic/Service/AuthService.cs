
using Common.Models;
using Microsoft.AspNetCore.Http;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic.Service
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContext;
        private UserInfo _sessionData = null;
        public AuthService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetEmail()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.Email;
            }
            return string.Empty;
        }

        public string GetFullName()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.Fullname;
            }
            return string.Empty;
        }

        public int GetUserId()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.AccountId;
            }
            return 0;
        }

        public UserInfo GetUserInfo()
        {
            UserInfo session = null;
            if (_sessionData != null)
            {
                session = _sessionData;
            }
            else
            {
                if (_httpContext != null && _httpContext.HttpContext != null)
                {
                    var context = _httpContext.HttpContext.Items["User"];
                    if (context != null && context is UserInfo)
                    {
                        session = context as UserInfo;
                    }
                }
            }
            return session;
        }

        public bool IsAdmin()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.Role == (int)Role.Admin;
            }
            return false;
        }

        public bool IsAuthenticated()
        {
            var authenticated = _httpContext?.HttpContext?.Request?.Headers?.ContainsKey("Authorization");
            return authenticated.GetValueOrDefault();
        }
    }
}
