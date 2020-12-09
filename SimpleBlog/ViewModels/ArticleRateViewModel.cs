using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class ArticleRateViewModel
    {
        public int ArticleId { get; set; }
        public bool IsRated { get; set; }
        public int RateValue { get; set; }
        public double RateAvarage { get; set; }
        public int VotesCount { get; set; }
    }
}