using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleTagListViewModel
    {
        public IEnumerable<Article> ArticlesList { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlesPhotosList { get; set; }
        public IEnumerable<ArticlesCategory> ArticlesCategoryList { get; set; }
        public int Page { get; set; }
        public int AllPages { get; set; }
        public int ArticlesTagId { get; set; }
        public int[] ArticlesCommentsCount { get; set; }
        public string TagName { get; set; }
    }
}