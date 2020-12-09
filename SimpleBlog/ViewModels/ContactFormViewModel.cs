using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ContactFormViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string MessageText { get; set; }
        public string GoogleRecaptchaV3 { get; set; }
        public string ValidationMsn { get; set; }
    }
}