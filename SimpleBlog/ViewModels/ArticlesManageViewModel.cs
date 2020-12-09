using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticlesManageViewModel
    {
        public IEnumerable<Article> ArticlesList { get; set; }
    }
}