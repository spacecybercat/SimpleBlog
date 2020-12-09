using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class IndexByCategoryViewModel
    {
        public int PageNumber { get; set; }
        public int ArticlesCategoryId { get; set; }
    }
}