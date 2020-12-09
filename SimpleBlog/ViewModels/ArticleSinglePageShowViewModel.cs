using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SimpleBlog.Models.IdentityModels;

namespace SimpleBlog.ViewModels
{
    public class ArticleSinglePageShowViewModel
    {
        public Article ArticleMain { get; set; }
        public Article ArticlePrev { get; set; }
        public Article ArticleNext { get; set; }
        public IEnumerable<Article> ArticlesRelated { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlePhotosMain { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlesPhotoRelated { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlesPhotoPrevNext { get; set; }
        public IEnumerable<ArticlesCategory> ArticlesCategoriesList { get; set; }
        public IEnumerable<ArticlesTag> ArticleTags { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }

    }
}