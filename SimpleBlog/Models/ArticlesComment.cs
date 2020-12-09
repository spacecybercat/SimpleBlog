using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class ArticlesComment
    {
        public int ArticlesCommentId { get; set; }
        public int ArticlesCommentPartenId { get; set; }
        public int ArticleId { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Wprowadź komentarz.")]
        [StringLength(1000)]
        public string Text { get; set; }
        public DateTime DateOfPublish { get; set; }
        public bool Visible { get; set; }
        public string GoogleRecaptchaV3 { get; set; }
    }
}