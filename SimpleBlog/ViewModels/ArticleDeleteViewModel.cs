using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleDeleteViewModel
    {
        public int ArticleId { get; set; }
        public int Comments { get; set; }
        public int CommentsRates { get; set; }
        public int Photos { get; set; }
        public int Rates { get; set; }
        public int TagConnections { get; set; }
        public bool Success { get; set; }
    }
}