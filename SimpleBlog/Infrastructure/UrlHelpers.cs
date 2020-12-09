using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Infrastructure
{
    public static class UrlHelpers
    {
        public static string ArticleImagePath(this UrlHelper helper, string articleImageFileName)
        {
            var ArticlesImagesFolder = AppConfig.ArticlesImagesFolderRelated;
            var path = Path.Combine(ArticlesImagesFolder, articleImageFileName);
            var pathUnrelated = helper.Content(path);
            return pathUnrelated;
        }
        public static string GetTitleLink(string Title)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(Title);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            asciiStr = asciiStr.ToString().Replace(' ', '_');
            asciiStr = asciiStr.ToString().Replace('.', '_');
            asciiStr = asciiStr.ToString().Replace('"', '_');
            asciiStr = asciiStr.ToString().Replace('/', '_');
            asciiStr = asciiStr.ToString().Replace('\\', '_');
            return asciiStr;
        }
    }
}