using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class InstallationViewModel
    {
        //1 - newlocal, 2 - local, 3 - new server, 4 - server
        public int DatabaseOption { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseAdminLogin { get; set; }
        public string DatabaseAdminPassword { get; set; }
        public string DatabaseServer { get; set; }
        public string SimpleBlogAdminLogin { get; set; }
        public string SimpleBlogAdminPassword { get; set; }
        public string ResultMessage { get; set; }
    }
}