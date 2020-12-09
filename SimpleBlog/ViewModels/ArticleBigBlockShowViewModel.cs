using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleBigBlockShowViewModel
    {
        public IEnumerable<Article> ArticlesList { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlesPhotosList { get; set; }
        public int[] ArticlesComments { get; set; }
    }
}