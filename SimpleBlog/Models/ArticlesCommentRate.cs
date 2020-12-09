using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class ArticlesCommentRate
    {
        public int ArticlesCommentRateId { get; set; }
        public int ArticlesCommentId { get; set; }
        public int ArticleId { get; set; }
        public int Value { get; set; }
        public int UserId { get; set; }
    }
}