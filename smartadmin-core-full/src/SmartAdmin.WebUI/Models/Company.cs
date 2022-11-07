using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class Company
    {
        [Key]
        public int n_company_id { get; set; }
        public string s_company_name { get; set; }
    }
}
