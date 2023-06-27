using Common.Models;

namespace Common.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJwtHelper
    {
        public string GenerateToken(UserInfo account);
        public UserInfo? ValidateToken(string token);
    }
}
