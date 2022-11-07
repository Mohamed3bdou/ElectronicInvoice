using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Models;

namespace SmartAdmin.WebUI.Data
{
    public class ConfigDbContext : IdentityDbContext
    {
        public ConfigDbContext(DbContextOptions<ConfigDbContext> options)
            : base(options)
        {
        }

        public DbSet<CompanyInfo> CompanyInfo { get; set; }
        public DbSet<Company> Company { get; set; }

    }
}
