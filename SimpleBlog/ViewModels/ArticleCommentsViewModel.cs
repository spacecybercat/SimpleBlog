using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleCommentsViewModel
    {
        public int ArticleId { get; set; }
        public IEnumerable<ArticlesComment> ArticleCommentsList { get; set; }
        public string ValidationMsn { get; set; }
    }
}