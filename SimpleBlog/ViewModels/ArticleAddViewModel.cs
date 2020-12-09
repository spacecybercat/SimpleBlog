using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleAddViewModel
    {
        public Article Article { get; set; }
        public string Tags { get; set; }
        public List<ArticlesTag> TagsList { get; set; }
        public List<Classes.TagCheck> TagsListChecked { get; set; }
        public IEnumerable<ArticlesPhoto> ArticlePhotos { get; set; }
        public ArticlesPhoto Photo1300x722 { get; set; }
        public ArticlesPhoto Photo1024x569 { get; set; }
        public ArticlesPhoto Photo768x339 { get; set; }
        public ArticlesPhoto Photo502x264 { get; set; }
        public ArticlesPhoto Photo312x223 { get; set; }
        public ArticlesPhoto Photo300x167 { get; set; }
        public ArticlesPhoto Photo117x84 { get; set; }
        public ArticlesPhoto Photo90x72 { get; set; }
        public IEnumerable<ArticlesCategory> ArticlesCategories { get; set; }
        public bool? Confirmation { get; set; }
    }
}