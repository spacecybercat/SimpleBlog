using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class FooterViewModel
    {
        public IEnumerable<Article> RecentArticles { get; set; }
        public IEnumerable<ArticlesPhoto> RecentArticlesPhotos { get; set; }
        public IEnumerable<ArticlesComment> RecentComments { get; set; }
        public IEnumerable<Article> CommentedArticles { get; set; }
    }
}