using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdmin.WebUI.App_Helpers
{
    public enum Status
    {
        None = 0,
        Success = 1,
        Failed = 2,
        SuccessWithWarning = 3
    }
    public class Logger
    {
        public dynamic TempId { get; set; }
        public int StatusCode
        {
            get
            {
                return (int)status;
            }
        }

        public Status status { get; set; }

        public string UserMessage { get; set; }

        public string ErrorMessage { get; set; }
    }
}
