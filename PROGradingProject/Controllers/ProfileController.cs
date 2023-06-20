using Common.Models;
using DataAccess.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PROGradingAPI.Controllers
{
    [Authorize]
    public class ProfileController : BaseNewController
    {
        AppDbContext _context;
        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all accounts
        /// </summary>r
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            return Ok();
        }

    }
}
