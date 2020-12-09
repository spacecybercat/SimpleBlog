using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class ArticlesPhoto
    {
        public int ArticlesPhotoId { get; set; }
        public string FilePath { get; set; }
        public int ArticleId { get; set; }
        public int OrderNo { get; set; }
        public bool MasterPhoto { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}