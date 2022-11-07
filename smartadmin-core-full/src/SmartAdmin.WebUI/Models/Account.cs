using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartAdmin.WebUI.Models
{
    public class CompanyInfo
    {
        [Key]
        public int n_id { get; set; }
        public int n_comp_id { get; set; }
        public string s_user_name { get; set; }
        public string s_user_password { get; set; }
        public string s_db_name { get; set; }
        public string s_db_user { get; set; }
        public string s_db_password { get; set; }
        public string s_server { get; set; }
        public string s_external_server { get; set; }
    }
}
