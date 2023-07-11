using BusinessLogic;
using Common.Attributes;
using Common.Helpers;
using Common.Models;
using DataAccess.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROGradingAPI.Controllers.Base;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{

    public class AccountController : BaseNewController
    {
        AppDbContext _context;
        private readonly IJwtHelper _jwtUltil;
        private readonly IAccountService _acc;


        public AccountController(AppDbContext context, IJwtHelper jwtUltil, IAccountService acc, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(httpContextAccessor, configuration)
        {
            _acc = acc;
            _jwtUltil = jwtUltil;
            _context = context;

        }

        /// <summary>
        /// Register account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDTO account)
        {
            ServiceResponse response = new ServiceResponse();
            if (!ModelState.IsValid)
            {
                // convert model state error to string 
                Dictionary<string, string[]> errors = new Dictionary<string, string[]>();
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        errors.Add(item.Key, item.Value.Errors.Select(x => x.ErrorMessage).ToArray());
                    }
                }
                response.OnError(message: "Invalid data", data: errors);
                return StatusCode(200, response);
            }
            var result = _acc.Register(account);
            if (result != null)
            {
                return StatusCode(201, result);
            }
            else
            {
                return StatusCode(400, "Register failed");
            }
        }

        /// <summary>
        /// Login account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _acc.Login(request);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    message = "Username or password is incorrect"
                });
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public IActionResult Delete()
        {
            var result = _acc.DeleteUser();
            if (result != null)
            {
                return StatusCode(200, result);
            }
            else
            {
                return StatusCode(400, "Delete failed");
            }
        }

        /// <summary>
        /// Update account
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public IActionResult Update([FromBody] AccountRequestDTO account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _acc.Update(account);
            if (result != null)
            {
                return StatusCode(200, result);
            }
            else
            {
                return StatusCode(400, "Update failed");
            }
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPut("changepassword")]

        public IActionResult UpdatePassword([FromBody] UpdatePasswordRequestDTO account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _acc.UpdatePassword(account.OldPassword, account.NewPassword);
            if (result != null)
            {
                return StatusCode(200, result);
            }
            else
            {
                return StatusCode(400, "Update failed");
            }
        }

        /// <summary>
        /// Get account by id
        /// </summary>
        /// <param name="accId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            return StatusCode(200, _acc.GetUserInfor());
        }

        /// <summary>
        /// Update multiple roles
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        [CustomAuthorize(Role.Admin)]
        [HttpPut("role")]
        public IActionResult UpdateRole([FromBody] List<RoleUpdate> roles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _acc.UpdateRole(roles);
            if (result != null)
            {
                return StatusCode(200, result);
            }
            else
            {
                return StatusCode(400, "Update failed");
            }
        }

        /// <summary>
        /// Check token
        /// </summary>
        /// <returns></returns>
        [HttpGet("checkToken")]
        [Authorize]
        public IActionResult CheckToken()
        {
            var token = _httpContext.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = _jwtUltil.ValidateToken(token);
            if (result != null)
            {
                return StatusCode(200, _acc.GetById());
            }
            else
            {
                return StatusCode(400, "Token invalid");
            }
        }
    }
}

