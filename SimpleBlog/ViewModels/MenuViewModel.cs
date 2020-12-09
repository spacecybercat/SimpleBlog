using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class MenuViewModel
    {
        public IEnumerable<ArticlesCategory> ArticlesCategories { get; set; }
        public int ActiveSubmenu { get; set; }
    }
}