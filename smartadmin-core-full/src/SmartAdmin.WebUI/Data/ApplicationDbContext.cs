using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Models;

namespace SmartAdmin.WebUI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public string ConnectionString { get; set; }

        public ApplicationDbContext(string databaseConnection)
            : base()
        {
            ConnectionString = databaseConnection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public DbSet<ar_customers> ar_customers { get; set; }
    }
}
