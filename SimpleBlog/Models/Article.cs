using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string AuthorId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string IntroText { get; set; }
        public string Text { get; set; }
        public DateTime DateOfPublish { get; set; }
        public bool Visible { get; set; }
        public int Views { get; set; }
        public string VideoLink { get; set; }
        public bool AllowComments { get; set; }
        public bool AllowRates { get; set; }
    }
}