using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using SmartAdmin.WebUI.App_Helpers;
namespace SmartAdmin.WebUI.Controllers
{
    public class CustomerController : BaseController
    {
        public CustomerController(ConfigDbContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment) : base(context, config, webHostEnvironment)
        {
        }

       [HttpGet]
        public IActionResult customerCreate()
        {
            var data = _contextFactory.ar_customers.Where(x => x.n_customer_id == 50).FirstOrDefault();
            return View("customerCreate",data);
        }

    }
}
