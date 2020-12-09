using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticlesCategoriesManageViewModel
    {
        public IEnumerable<ArticlesCategory> ArticleCategories { get; set; }
        public string ArticlesCategoryAddName { get; set; }
    }
}