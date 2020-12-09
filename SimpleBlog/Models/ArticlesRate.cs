using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class ArticlesRate
    {
        public int ArticlesRateId { get; set; }
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public int Value { get; set; }
    }
}