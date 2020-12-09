using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleCommentRateViewModel
    {
        public int ArticleCommentId { get; set; }
        public int ArticleCommentLikesCount { get; set; }
        public int ArticleCommentDislikesCount { get; set; }
        public int ArticleCommentRateValue { get; set; }

    }
}