using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class ArticlesTagsConnection
    {
        public int ArticlesTagsConnectionId { get; set; }
        public int ArticleId { get; set; }
        public int ArticlesTagId { get; set; }
    }
}