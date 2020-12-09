using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class TrendingArticlesViewModel
    {
        public List<Article> TrendingArticles { get; set; }
    }
}