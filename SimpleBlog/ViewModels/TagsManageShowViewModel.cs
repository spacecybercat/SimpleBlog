using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class TagsManageShowViewModel
    {
        public List<ArticlesTag> TagsList { get; set; }
        public List<ArticlesTagsConnection> TagsConnectionsList { get; set; }
    }
}