using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{

    public class ar_customers
    {
        [Key]
        public int n_customer_id { get; set; }
        public int n_DataAreaID { get; set; }
        public string s_customer_name { get; set; }
        public string s_customer_name_eng { get; set; }
        public Nullable<int> n_customer_type_id { get; set; }
        public Nullable<int> n_taxes_type { get; set; }
        public string s_customer_address { get; set; }
        public string s_customer_phone_no { get; set; }
        public string s_customer_e_mail { get; set; }
        public string s_customer_fax_no { get; set; }
        public string d_UserUpdateDate { get; set; }
        public Nullable<int> n_UserUpdate { get; set; }
        public string d_UserAddDate { get; set; }
        public Nullable<int> n_UserAdd { get; set; }
        public Nullable<bool> b_Deleted { get; set; }
        public Nullable<int> n_customer_nature { get; set; }
        public string s_tax_file_no { get; set; }
        public Nullable<int> n_tax_office { get; set; }
        public string s_vat_no { get; set; }
        public string s_country { get; set; }
        public string s_governorate { get; set; }
        public string s_city { get; set; }
        public string s_building_number { get; set; }
        public string s_street_name { get; set; }
        public string s_zipcode { get; set; }
        public string s_floor { get; set; }
        public string s_room { get; set; }
        public string s_landmark { get; set; }
        public string s_addtional { get; set; }

    }
}
