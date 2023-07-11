using Common.Models;
using Microsoft.AspNetCore.Http;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContext;
        private UserInfo _sessionData = null;
        public AuthService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.Email;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.Fullname;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetUserId()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.AccountId;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            var session = GetUserInfo();
            if (session != null)
            {
                return session.Role == (int)Role.Admin;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            var authenticated = _httpContext?.HttpContext?.Request?.Headers?.ContainsKey("Authorization");
            return authenticated.GetValueOrDefault();
        }
    }
}
