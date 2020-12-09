using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using static SimpleBlog.Models.IdentityModels;
using SimpleBlog.Models;

namespace SimpleBlog.DataAccessLayer
{
    public class SimpleBlogContext : IdentityDbContext<ApplicationUser>
    {
        public SimpleBlogContext() : base("SimpleBlog")
        {

        }
        public DbSet<ArticlesCategory> ArticlesCategories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticlesComment> ArticlesComments { get; set; }
        public DbSet<ArticlesCommentRate> ArticlesCommentsRates { get; set; }
        public DbSet<ArticlesPhoto> ArticlesPhotos { get; set; }
        public DbSet<ArticlesRate> ArticlesRates { get; set; }
        public DbSet<ArticlesTag> ArticlesTags { get; set; }
        public DbSet<ArticlesTagsConnection> ArticlesTagsConnections { get; set; }


        public static SimpleBlogContext Create()
        {
            return new SimpleBlogContext();
        }

    }
}