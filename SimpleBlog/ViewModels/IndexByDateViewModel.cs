using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class IndexByDateViewModel
    {
        public int PageNumber { get; set; }
        public string Date { get; set; }
    }
}