using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Common.Helpers
{
    /// <summary>
    /// JwtUtils
    /// </summary>
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string GenerateToken(UserInfo userInfo)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtExpireMinutes = _configuration["Jwt:ExpireMinutes"];
            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtExpireMinutes))
            {
                throw new Exception("Jwt key or expire minutes is not configured");
            }
            var tokenHandler = new JwtSecurityTokenHandler(); // create token handler
            var secretKey = Encoding.ASCII.GetBytes(jwtKey); // get secret key from appsettings.json
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.AccountId.ToString()),
                    new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.MobilePhone, userInfo.Phone),
                    new Claim(ClaimTypes.StreetAddress, userInfo.Address),
                    new Claim(ClaimTypes.Name, userInfo.Fullname),
                    new Claim("code", userInfo.Code),
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtExpireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserInfo? ValidateToken(string token)
        {
            if (token == null) return null;
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("Jwt key is not configured");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "");
                var accountRole = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.Role)?.Value ?? "");
                var accountEmail = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email)?.Value;
                var accountPhone = jwtToken.Claims.First(x => x.Type == ClaimTypes.MobilePhone)?.Value;
                var accountAddress = jwtToken.Claims.First(x => x.Type == ClaimTypes.StreetAddress)?.Value;
                var accountFullname = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name)?.Value;
                var accountCode = jwtToken.Claims.First(x => x.Type == "code")?.Value;
                return new UserInfo()
                {
                    AccountId = accountId,
                    Role = accountRole,
                    Email = accountEmail ?? "",
                    Phone = accountPhone ?? "",
                    Address = accountAddress ?? "",
                    Fullname = accountFullname ?? "",
                    Code = accountCode ?? "",
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
