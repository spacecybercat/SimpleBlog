using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticlesMainListViewModel
    {
        public IEnumerable<Article> ArticlesList { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlesPhotosList { get; set; }
        public IEnumerable<ArticlesCategory> ArticlesCategoriesList { get; set; }
        public int Page { get; set; }
        public int AllPages { get; set; }
        public int[] ArticlesCommentsCount { get; set; }
    }
}