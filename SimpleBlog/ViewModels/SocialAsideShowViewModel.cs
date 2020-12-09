using SimpleBlog.Classes;
using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class SocialAsideShowViewModel
    {
        public IEnumerable<ArticlesTag> PopularTagsList { get; set; }
        public IEnumerable<TagRelationCounter> TagCounter { get; set; }
    }
}