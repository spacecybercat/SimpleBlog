using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class SearchResultIndexViewModel
    {
        public int PageNumber { get; set; }
        public string SearchText { get; set; }
    }
}