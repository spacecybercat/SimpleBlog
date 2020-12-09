using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SimpleBlog.Infrastructure
{
    public class AppConfig
    {

        private static string _articlesImagesFolderRelated = ConfigurationManager.AppSettings["ArticlesImagesFolder"];

        public static string ArticlesImagesFolderRelated
        {
            get
            {
                return _articlesImagesFolderRelated;
            }
        }

        private static string _announcmentsImagesFolderRelated = ConfigurationManager.AppSettings["AnnouncmentsImagesFolder"];

        public static string AnnouncmentsImagesFolderRelated
        {
            get
            {
                return _announcmentsImagesFolderRelated;
            }
        }

        private static string _usersAvatarsRelated = ConfigurationManager.AppSettings["UsersAvatarsFolder"];

        public static string UsersAvatarsFolderRelated
        {
            get
            {
                return _usersAvatarsRelated;
            }
        }

        private static string _advertisementImagesFolderRelated = ConfigurationManager.AppSettings["AdvertisementImagesFolder"];

        public static string AdvertisementImagesFolderRelated
        {
            get
            {
                return _advertisementImagesFolderRelated;
            }
        }
    }
}