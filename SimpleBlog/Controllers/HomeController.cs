using SimpleBlog.DataAccessLayer;
using SimpleBlog.ViewModels;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using static SimpleBlog.Models.IdentityModels;
using System.Collections.Generic;
using SimpleBlog.Models;
using SimpleBlog.Classes;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Net.Http;

namespace SimpleBlog.Controllers
{
    public class HomeController : Controller
    {
        private SimpleBlogContext dataBase = new SimpleBlogContext();

        public ActionResult Index(int? pageNumber)
        {
            if (dataBase.Database.Exists())
            {
                int pageNo = 1;
                if (pageNumber.HasValue)
                {
                    pageNo = pageNumber.Value;
                    if (pageNo <= 1) pageNo = 1;
                }
                var VM = new IndexViewModel { PageNumber = pageNo };
                return View(VM);
            }
            else
            {
                var InstallVM = new InstallationViewModel();
                return RedirectToAction("Installation", InstallVM);
            }
        }
        public ActionResult Installation(InstallationViewModel InstallationVM, int? DatabaseOption)
        {
            if (DatabaseOption.HasValue)
            {
                InstallationVM.DatabaseOption = DatabaseOption.Value;
            }
            else
            {
                InstallationVM.DatabaseOption = 0;
            }
            return View("Installation", InstallationVM);
        }
        [HttpPost]
        public ActionResult Installation(InstallationViewModel InstallationVM)
        {
            if (InstallationVM.DatabaseOption == 1 || InstallationVM.DatabaseOption == 2)
            {
                InstallationVM.DatabaseServer = "(localdb)\\MSSQLLocalDB";
                if (InstallationVM.DatabaseOption == 1)
                {
                    InstallationVM.DatabaseName = "SimpleBlog";
                }
            }
            var connectionState = AddConnectionString("SimpleBlog", InstallationVM);

            if (connectionState == "Success")
            {
                return RedirectToAction("InstallationCompleted", InstallationVM);
            }
            else
            {
                InstallationVM.ResultMessage = connectionState;
                return RedirectToAction("Installation", InstallationVM);
            }
        }
        public ActionResult IstallationChangeServerOption(int DatabaseOption)
        {
            var InstallVM = new InstallationViewModel();
            InstallVM.DatabaseOption = DatabaseOption;
            return RedirectToAction("Installation", InstallVM);
        }
        private string AddConnectionString(string name, InstallationViewModel InstallationVM)
        {
            bool isNew = false;
            string path = Server.MapPath("~/Web.Config");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("connectionStrings/add[@name='{0}']", name));
            XmlNode node;
            isNew = list.Count == 0;
            if (isNew)
            {
                node = doc.CreateNode(XmlNodeType.Element, "add", null);
                XmlAttribute attribute = doc.CreateAttribute("name");
                attribute.Value = name;
                node.Attributes.Append(attribute);

                attribute = doc.CreateAttribute("connectionString");
                attribute.Value = "";
                node.Attributes.Append(attribute);

                attribute = doc.CreateAttribute("providerName");
                attribute.Value = "System.Data.SqlClient";
                node.Attributes.Append(attribute);
            }
            else
            {
                node = list[0];
            }
            string conString = node.Attributes["connectionString"].Value;
            SqlConnectionStringBuilder conStringBuilder = new SqlConnectionStringBuilder(conString);
            conStringBuilder.InitialCatalog = InstallationVM.DatabaseName;
            conStringBuilder.DataSource = InstallationVM.DatabaseServer;
            conStringBuilder.IntegratedSecurity = false;
            if (InstallationVM.DatabaseOption == 3 || InstallationVM.DatabaseOption == 4)
            {
                conStringBuilder.UserID = InstallationVM.DatabaseAdminLogin;
                conStringBuilder.Password = InstallationVM.DatabaseAdminPassword;
            }
            else
            {
                conStringBuilder.IntegratedSecurity = true;
                conStringBuilder.TrustServerCertificate = false;
                if (InstallationVM.DatabaseOption == 1)
                {
                    SimpleBlogContext dataBase = new SimpleBlogContext();
                    dataBase.Database.CreateIfNotExists();
                }
            }
            node.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;
            if (isNew)
            {
                doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(node);
            }

            try
            {
                SqlConnection sqlConnection = new SqlConnection(conStringBuilder.ConnectionString);
                sqlConnection.Open();
                doc.Save(path);
                return "Success";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
        public ActionResult InstallationCompleted(InstallationViewModel InstallationVM)
        {
            InstallationCompletedViewModel VM = new InstallationCompletedViewModel();
            VM.Message = CreadteAdminAccountAndRole(InstallationVM);
            return View(VM);
        }
        public string CreadteAdminAccountAndRole(InstallationViewModel InstallationVM)
        {
            try
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dataBase));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dataBase));
                var user = userManager.FindByName(InstallationVM.SimpleBlogAdminLogin.ToString());
                if (user == null)
                {
                    if (InstallationVM.SimpleBlogAdminPassword.Length < 6)
                    {
                        dataBase.Database.Delete();
                        return "Instalacja zakończona niepowodzeniem z błędem: Hasło admina SimpleBlog musi składać się z przynajmniej 6 znaków!";
                    }
                    user = new ApplicationUser { Nickname = "Redaktor", UserName = InstallationVM.SimpleBlogAdminLogin };
                    var result = userManager.Create(user, InstallationVM.SimpleBlogAdminPassword);
                }
                var role = roleManager.FindByName("Admin");
                if (role == null)
                {
                    role = new IdentityRole("Admin");
                    roleManager.Create(role);
                }
                var rolesOfUser = userManager.GetRoles(user.Id);
                if (!rolesOfUser.Contains(role.Name))
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                return "Instalacja zakończona sukcesem!";
            }
            catch (Exception e)
            {
                return "Instalacja zakończona niepowodzeniem z błędem: " + e.Message;
            }
        }


        public ActionResult IndexByCategory(int? pageNumber, int articlesCategoryId)
        {
            int pageNo = 1;
            if (pageNumber.HasValue)
            {
                pageNo = pageNumber.Value;
                if (pageNo <= 1) pageNo = 1;
            }
            var VM = new IndexByCategoryViewModel { PageNumber = pageNo, ArticlesCategoryId = articlesCategoryId };
            return View(VM);
        }
        public ActionResult IndexByTag(int? pageNumber, int articlesTagId)
        {
            int pageNo = 1;
            if (pageNumber.HasValue)
            {
                pageNo = pageNumber.Value;
                if (pageNo <= 1) pageNo = 1;
            }
            var VM = new IndexByTagViewModel { PageNumber = pageNo, TagId = articlesTagId };
            return View(VM);
        }
        public ActionResult IndexByDate(int? pageNumber, string date)
        {
            int pageNo = 1;
            if (pageNumber.HasValue)
            {
                pageNo = pageNumber.Value;
                if (pageNo <= 1) pageNo = 1;
            }
            var VM = new IndexByDateViewModel { PageNumber = pageNo, Date = date };
            return View(VM);
        }
        public ActionResult MainMenuShow(int activeSubmenu)
        {
            MenuViewModel VM = new MenuViewModel { ActiveSubmenu = activeSubmenu, ArticlesCategories = dataBase.ArticlesCategories };
            return PartialView("_MainMenuShow", VM);
        }
        public ActionResult SideMenuShow()
        {
            List<ArticlesCategory> articlesCategories = dataBase.ArticlesCategories.ToList();
            return PartialView("_SideMenuShow", articlesCategories);
        }
        public ActionResult BackToTopShow()
        {
            return PartialView("_BackToTopShow");
        }
        public ActionResult FooterShow()
        {
            //ostatnio dodane artykuły
            var recentArticles = dataBase.Articles.Where(v => v.Visible == true).OrderByDescending(d => d.DateOfPublish).Take(3).ToList();
            List<ArticlesPhoto> recentArticlesPhotos = new List<ArticlesPhoto>();
            foreach (var article in recentArticles)
            {
                recentArticlesPhotos.Add(dataBase.ArticlesPhotos.Where(id => id.ArticleId == article.ArticleId).ToList().First());
            }
            /////////////////////////

            //ostatnio dodane komentarze - lista 4 artykułów
            var recentComments = dataBase.ArticlesComments.Where(v => v.Visible == true).OrderByDescending(d => d.DateOfPublish).ToList();
            List<Article> commentedArticles = new List<Article>();
            foreach (var comment in recentComments)
            {
                if (!commentedArticles.Contains(dataBase.Articles.Where(id => id.ArticleId == comment.ArticleId).First()))
                {
                    commentedArticles.Add(dataBase.Articles.Where(id => id.ArticleId == comment.ArticleId).First());
                }
            }
            ///////////////////////////////////////////////////


            FooterViewModel VM = new FooterViewModel { RecentArticles = recentArticles, RecentArticlesPhotos = recentArticlesPhotos, RecentComments = recentComments, CommentedArticles = commentedArticles.Take(4)};
            return PartialView("_FooterShow", VM);
        }
        public ActionResult TrendingArticlesShow()
        {
            List<Article> trendingArticles = new List<Article>();
            var allArticles = dataBase.Articles.Where(v => v.Visible == true).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();

            foreach (var article in allArticles)
            {
                if (article.DateOfPublish.Date > DateTime.Now.Date.AddDays(-30))
                {
                    trendingArticles.Add(article);
                }
            }
            trendingArticles = trendingArticles.OrderByDescending(v => v.Views).Take(3).ToList();
            TrendingArticlesViewModel VM = new TrendingArticlesViewModel { TrendingArticles = trendingArticles };
            return PartialView("_TrendingArticlesShow", VM);
        }
        public ActionResult BreadcrumbsShow()
        {
            return PartialView("_BreadcrumbsShow");
        }
        public ActionResult SocialAsideShow()
        {

            var tagCounter = new List<TagRelationCounter>();
            var tags = dataBase.ArticlesTags.ToList();
            var tagConnections = dataBase.ArticlesTagsConnections.ToList();
            foreach (var tag in tags)
            {
                var TC = new TagRelationCounter();
                TC.ArticleId = tag.ArticlesTagId;
                TC.Counter = tagConnections.Where(tid => tid.ArticlesTagId == tag.ArticlesTagId).Count();
                tagCounter.Add(TC);
            }
            tagCounter = tagCounter.OrderByDescending(c => c.Counter).Take(10).ToList();

            var VM = new SocialAsideShowViewModel { PopularTagsList = tags, TagCounter = tagCounter};
            return PartialView("_SocialAsideShow", VM);
        }
        public ActionResult ArticleBigBlockShow()
        {
            var articleList = dataBase.Articles.OrderByDescending(o => o.DateOfPublish).Where(v => v.Visible == true).Where(ad => ad.DateOfPublish < DateTime.Now).Take(6).ToList();
            var articlesPhotos = new List<ArticlesPhoto>();
            int[] articlesComments = new int[6];
            int i = 0;
            foreach (var article in articleList)
            {
                if (articleList.Count == 6)
                {
                    articlesComments[i] = dataBase.ArticlesComments.Where(id => id.ArticleId == article.ArticleId).Count();
                    i++;
                    var photo = dataBase.ArticlesPhotos.Where(a => a.ArticleId == article.ArticleId).First();
                    articlesPhotos.Add(photo);
                    var photo2 = dataBase.ArticlesPhotos.Where(a => a.ArticleId == article.ArticleId).First();
                    articlesPhotos.Add(photo2);
                }
            }
            ArticleBigBlockShowViewModel VM = new ArticleBigBlockShowViewModel { ArticlesList = articleList, ArticlesPhotosList = articlesPhotos, ArticlesComments = articlesComments };
            return PartialView("_ArticleBigBlockShow", VM);
        }
        public ActionResult ArticlesMainListShow(int? page)
        {
            int ArticlesPerPage = 10;
            var articlesList = dataBase.Articles.OrderByDescending(d => d.DateOfPublish).Where(v => v.Visible == true).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();
            var articlesListPaged = new List<Article>();
            var articlesPhotosPaged = new List<ArticlesPhoto>();
            int[] articlesCommentsCount = new int[ArticlesPerPage];
            int currentPage = 1;
            int pagesCount = (articlesList.Count - 6) / ArticlesPerPage;
            if (((articlesList.Count - 6) % ArticlesPerPage) != 0) pagesCount++;
            if (page.HasValue)
            {
                if (page > pagesCount) page = pagesCount;
                currentPage = page.Value;
                int elementNo = page.Value * ArticlesPerPage - ArticlesPerPage + 6;
                for (int i = 0; i < ArticlesPerPage; i++)
                {
                    if (elementNo < articlesList.Count && elementNo >= 0)
                    {
                        articlesListPaged.Add(articlesList.ElementAt(elementNo));
                    }
                    elementNo++;
                }
                foreach (var article in articlesList)
                {
                    var photo = dataBase.ArticlesPhotos.Where(a => a.ArticleId == article.ArticleId).First();
                    articlesPhotosPaged.Add(photo);

                }
                int j = 0;
                foreach (var article in articlesListPaged)
                {
                    articlesCommentsCount[j] = dataBase.ArticlesComments.Where(id => id.ArticleId == article.ArticleId).Count();
                    j++;
                }
            }
            ArticlesMainListViewModel VM = new ArticlesMainListViewModel { ArticlesList = articlesListPaged, ArticlesPhotosList = articlesPhotosPaged, AllPages = pagesCount, Page = currentPage, ArticlesCategoriesList = dataBase.ArticlesCategories, ArticlesCommentsCount = articlesCommentsCount};
            return PartialView("_ArticlesMainListShow", VM);
        }
        public ActionResult ArticlesListByCategoryShow(int ArticlesCategoryId, int? page)
        {
            int ArticlesPerPage = 10;
            var articlesList = dataBase.Articles.OrderByDescending(d => d.DateOfPublish).Where(v => v.Visible == true).Where(c => c.CategoryId == ArticlesCategoryId).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();
            var articlesListPaged = new List<Article>();
            var articlesPhotosPaged = new List<ArticlesPhoto>();
            int[] articlesCommentsCount = new int[ArticlesPerPage];
            int currentPage = 1;
            int pagesCount = articlesList.Count / ArticlesPerPage;
            if ((articlesList.Count % ArticlesPerPage) != 0) pagesCount++;
            if (page.HasValue)
            {
                if (page > pagesCount) page = pagesCount;
                currentPage = page.Value;
                int elementNo = page.Value * ArticlesPerPage - ArticlesPerPage;
                for (int i = 0; i < ArticlesPerPage; i++)
                {
                    if (elementNo < articlesList.Count && elementNo >= 0)
                    {
                        articlesListPaged.Add(articlesList.ElementAt(elementNo));
                    }
                    elementNo++;
                }
                foreach (var article in articlesList)
                {
                    var photo = dataBase.ArticlesPhotos.Where(a => a.ArticleId == article.ArticleId).First();
                    articlesPhotosPaged.Add(photo);
                }
                int j = 0;
                foreach (var article in articlesListPaged)
                {
                    articlesCommentsCount[j] = dataBase.ArticlesComments.Where(id => id.ArticleId == article.ArticleId).Count();
                    j++;
                }
            }
            ArticleCategoryListViewModel VM = new ArticleCategoryListViewModel { ArticlesList = articlesListPaged, ArticlesPhotosList = articlesPhotosPaged, AllPages = pagesCount, Page = currentPage, ArticlesCategoryId = ArticlesCategoryId, ArticlesCategoryList = dataBase.ArticlesCategories, ArticlesCommentsCount = articlesCommentsCount};
            return PartialView("_ArticlesListByCategoryShow", VM);
        }
        public ActionResult Backoffice()
        {
            return View();
        }
        public ActionResult ArticleSinglePageShow(int ArticleId, string ArticleTitle)
        {
            Article articleMain = dataBase.Articles.Where(i => i.ArticleId == ArticleId).First();
            Article articlePrev;
            Article articleNext;
            List<ArticlesPhoto> articlePhotosMain = dataBase.ArticlesPhotos.Where(i => i.ArticleId == articleMain.ArticleId).ToList();
            List<ArticlesPhoto> articlesPrevNextPhoto = new List<ArticlesPhoto>();

            var articlesList = dataBase.Articles.OrderByDescending(d => d.DateOfPublish).Where(v => v.Visible == true).ToList();
            int mainArticleElementNo = articlesList.IndexOf(articleMain);
            if (mainArticleElementNo > 0)
            {
                articlePrev = articlesList.ElementAt(mainArticleElementNo - 1);
                articlesPrevNextPhoto.Add(dataBase.ArticlesPhotos.Where(i => i.ArticleId == articlePrev.ArticleId).Where(po => po.OrderNo == 0).First());
            }
            else
            {
                articlePrev = articlesList.ElementAt(articlesList.Count - 1);
                articlesPrevNextPhoto.Add(dataBase.ArticlesPhotos.Where(i => i.ArticleId == articlePrev.ArticleId).Where(po => po.OrderNo == 0).First());
            }
            if (mainArticleElementNo < articlesList.Count - 1)
            {
                articleNext = articlesList.ElementAt(mainArticleElementNo + 1);
                articlesPrevNextPhoto.Add(dataBase.ArticlesPhotos.Where(i => i.ArticleId == articleNext.ArticleId).Where(po => po.OrderNo == 0).First());
            }
            else
            {
                articleNext = articlesList.ElementAt(0);
                articlesPrevNextPhoto.Add(dataBase.ArticlesPhotos.Where(i => i.ArticleId == articleNext.ArticleId).Where(po => po.OrderNo == 0).First());
            }

            //wyszukiwanie tagów do artykułu
            List<ArticlesTagsConnection> tagsConnections = dataBase.ArticlesTagsConnections.Where(i => i.ArticleId == ArticleId).ToList();
            List<ArticlesTag> tags = new List<ArticlesTag>();
            foreach (ArticlesTagsConnection tagConnection in tagsConnections)
            {
                ArticlesTag tag = dataBase.ArticlesTags.Where(i => i.ArticlesTagId == tagConnection.ArticlesTagId).First();
                tags.Add(tag);
            }
            List<ArticlesPhoto> articlesRelatedPhotos = new List<ArticlesPhoto>();
            List<ArticlesPhoto> photos = dataBase.ArticlesPhotos.ToList();
            var articlesRelated = FindRelatedArticles(ArticleId);
            foreach (Article article in articlesRelated)
            {
                articlesRelatedPhotos.Add(photos.Where(i => i.ArticleId == article.ArticleId).First());
            }
            /////////////////////////////////////////////////////
            ArticleSinglePageShowViewModel VM = new ArticleSinglePageShowViewModel { ArticleMain = articleMain, ArticlePhotosMain = articlePhotosMain, ArticlesCategoriesList = dataBase.ArticlesCategories, ArticlePrev = articlePrev, ArticleNext = articleNext, ArticlesPhotoPrevNext = articlesPrevNextPhoto, ArticleTags = tags.OrderBy(n => n.Name), ArticlesRelated = articlesRelated, ArticlesPhotoRelated = articlesRelatedPhotos, Users = dataBase.Users.ToList() };
            AddArticleView(ArticleId);
            return View(VM);
        }
        public IEnumerable<Article> FindRelatedArticles(int ArticleId)
        {
            var futureArticles = dataBase.Articles.Where(ad => ad.DateOfPublish > DateTime.Now).ToList();
            var articleMain = dataBase.Articles.Where(aid => aid.ArticleId == ArticleId).First();
            //wiazanie tagami
            var allTagConnections = dataBase.ArticlesTagsConnections.ToList();
            var articleTagConnections = dataBase.ArticlesTagsConnections.Where(a => a.ArticleId == ArticleId).ToList();
            foreach (var FutureArt in futureArticles)
            {
                var tagConnectionsToRemove = allTagConnections.Where(aid => aid.ArticleId == FutureArt.ArticleId).ToList();
                foreach (var tag in tagConnectionsToRemove)
                {
                    allTagConnections.Remove(tag);
                }
            }

            List<TagRelationCounter> relationList = new List<TagRelationCounter>();
            foreach (var articleTagConnection in articleTagConnections)
            {
                foreach (var allTagConnection in allTagConnections.Where(tid => tid.ArticlesTagId == articleTagConnection.ArticlesTagId))
                {
                    if (allTagConnection.ArticleId != ArticleId)
                    {
                        bool relIsCounted = false;
                        foreach (var relation in relationList)
                        {
                            if (relation.ArticleId == allTagConnection.ArticleId)
                            {
                                relation.Counter++;
                                relIsCounted = true;
                            }
                        }
                        if (relIsCounted == false)
                        {
                            relationList.Add(new TagRelationCounter { ArticleId = allTagConnection.ArticleId, Counter = 1 });
                        }
                    }
                }
            }
            /////////////////////////////////////////////
            ///
            //uzupełnianie jeśli lista relacji jest niepełna

            if (relationList.Count() < 3)
            {
                var relationListcopy = relationList.ToList();
                var artListSameCategory = dataBase.Articles.Where(v => v.Visible == true).Where(ac => ac.CategoryId == articleMain.CategoryId).OrderByDescending(ad => ad.DateOfPublish).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();
                for (int i = 0; i < artListSameCategory.Count; i++)
                {
                    if (relationListcopy.Count == 0)
                    {
                        if (artListSameCategory[i].ArticleId != articleMain.ArticleId)
                        {
                            relationList.Add(new TagRelationCounter { ArticleId = artListSameCategory[i].ArticleId, Counter = 1 });
                        }
                    }
                    foreach (var relation in relationListcopy)
                    {
                        if (artListSameCategory[i].ArticleId != relation.ArticleId && artListSameCategory[i].ArticleId != articleMain.ArticleId)
                        {
                            relationList.Add(new TagRelationCounter { ArticleId = artListSameCategory[i].ArticleId, Counter = 1 });
                        }
                        if (relationList.Count() >= 3)
                        {
                            continue;
                        }
                    }
                    if (relationList.Count() >= 3)
                    {
                        continue;
                    }
                }
            }
            ////////////////////////////////////
            ///
            ///uzupełnienie awaryjne
            if (relationList.Count() < 3)
            {
                var relationListcopy = relationList.ToList();
                var artListSameCategory = dataBase.Articles.Where(v => v.Visible == true).OrderByDescending(ad => ad.DateOfPublish).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();
                for (int i = 0; i < artListSameCategory.Count; i++)
                {
                    if (relationListcopy.Count == 0)
                    {
                        if (artListSameCategory[i].ArticleId != articleMain.ArticleId)
                        {
                            relationList.Add(new TagRelationCounter { ArticleId = artListSameCategory[i].ArticleId, Counter = 1 });
                        }
                    }
                    foreach (var relation in relationListcopy)
                    {
                        if (artListSameCategory[i].ArticleId != relation.ArticleId && artListSameCategory[i].ArticleId != articleMain.ArticleId)
                        {
                            relationList.Add(new TagRelationCounter { ArticleId = artListSameCategory[i].ArticleId, Counter = 1 });
                        }
                        if (relationList.Count() >= 3)
                        {
                            break;
                        }
                    }
                    if (relationList.Count() >= 3)
                    {
                        break;
                    }
                }
            }
            ////////////////////////////////////////////
            ///
            relationList.OrderByDescending(c => c.Counter);
            var relationOrderedList = relationList.OrderByDescending(c => c.Counter).Take(3).ToList();
            List<Article> relatedArticlesToReturn = new List<Article>();
            if (relationOrderedList.Count() > 0)
            {
                int artId = relationOrderedList.ToList()[0].ArticleId;
                var art = dataBase.Articles.Where(i => i.ArticleId == artId).First();
                relatedArticlesToReturn.Add(art);
                if (relationOrderedList.Count() > 1)
                {
                    artId = relationOrderedList.ToList()[1].ArticleId;
                    art = dataBase.Articles.Where(i => i.ArticleId == artId).First();
                    relatedArticlesToReturn.Add(art);
                }
                if (relationOrderedList.Count() > 2)
                {
                    artId = relationOrderedList.ToList()[2].ArticleId;
                    art = dataBase.Articles.Where(i => i.ArticleId == artId).First();
                    relatedArticlesToReturn.Add(art);
                }
            }
            return relatedArticlesToReturn;
        }
        public int CountComments(int ArticleId)
        {
            int comments = dataBase.ArticlesComments.Where(c => c.ArticleId == ArticleId).Count();
            //CountCommentsViewModel VM = new CountCommentsViewModel { NoOfComments = comments };
            return comments;
        }
        public void AddArticleView(int ArticleId)
        {
            var article = dataBase.Articles.Where(i => i.ArticleId == ArticleId).First();
            article.Views++;
            dataBase.Entry(article).State = System.Data.Entity.EntityState.Modified;
            dataBase.SaveChanges();
        }
        public ActionResult ArticlesListByTagShow(int ArticlesTagId, int? page)
        {
            int ArticlesPerPage = 10;
            var articlesTagConnectionsByTag = dataBase.ArticlesTagsConnections.Where(i => i.ArticlesTagId == ArticlesTagId);
            var articlesConnectedByTag = new List<Article>();
            var allArticles = dataBase.Articles.Where(v => v.Visible == true).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();
            Article article = new Article();
            int[] articlesCommentsCount = new int[10];
            foreach (var articleConnection in articlesTagConnectionsByTag)
            {
                if (allArticles.Where(aid => aid.ArticleId == articleConnection.ArticleId).First().Visible == true)
                {
                    int artId = articleConnection.ArticleId;
                    article = allArticles.Where(i => i.ArticleId == artId).First();
                    articlesConnectedByTag.Add(article);
                }
            }
            var articlesList = articlesConnectedByTag.OrderByDescending(d => d.DateOfPublish).Where(v => v.Visible == true);
            var articlesListPaged = new List<Article>();
            var articlesPhotosPaged = new List<ArticlesPhoto>();
            int currentPage = 1;
            int pagesCount = articlesList.ToList().Count / ArticlesPerPage;
            if ((articlesList.ToList().Count % ArticlesPerPage) != 0) pagesCount++;
            if (page.HasValue)
            {
                if (page > pagesCount) page = pagesCount;
                currentPage = page.Value;
                int elementNo = page.Value * ArticlesPerPage - ArticlesPerPage;
                for (int i = 0; i < ArticlesPerPage; i++)
                {
                    if (elementNo < articlesList.ToList().Count && elementNo >= 0)
                    {
                        articlesListPaged.Add(articlesList.ElementAt(elementNo));
                    }
                    elementNo++;
                }
                foreach (Article articlee in articlesList)
                {
                    var photo = dataBase.ArticlesPhotos.Where(a => a.ArticleId == articlee.ArticleId).First();
                    articlesPhotosPaged.Add(photo);
                }
                int j = 0;
                foreach (Article articlee in articlesListPaged)
                {
                    articlesCommentsCount[j] = dataBase.ArticlesComments.Where(id => id.ArticleId == articlee.ArticleId).Count();
                    j++;
                }
            }
            var tagName = dataBase.ArticlesTags.Where(tid => tid.ArticlesTagId == ArticlesTagId).First();
            ArticleTagListViewModel VM = new ArticleTagListViewModel { ArticlesList = articlesListPaged, ArticlesPhotosList = articlesPhotosPaged, AllPages = pagesCount, Page = currentPage, ArticlesCategoryList = dataBase.ArticlesCategories, ArticlesTagId = ArticlesTagId, ArticlesCommentsCount = articlesCommentsCount, TagName = tagName.Name};
            return PartialView("_ArticlesListByTagShow", VM);
        }
        public ActionResult ArticlesListByDateShow(string Date, int? page)
        {
            int ArticlesPerPage = 10;
            //DateTime date = DateTime.Parse(Date);
            var allArticles = dataBase.Articles.Where(v => v.Visible == true).Where(ad => ad.DateOfPublish < DateTime.Now).ToList();
            List<Article> articlesList = new List<Article>();
            int[] articlesCommentsCount = new int[10];
            foreach (var article in allArticles)
            {
                string artDate = article.DateOfPublish.ToString("ddMMyyyy");
                if (Date == artDate)
                {
                    articlesList.Add(article);
                }
            }
            articlesList = articlesList.OrderByDescending(d => d.DateOfPublish).ToList();
            var articlesListPaged = new List<Article>();
            var articlesPhotosPaged = new List<ArticlesPhoto>();
            int currentPage = 1;
            int pagesCount = articlesList.Count / ArticlesPerPage;
            if ((articlesList.Count % ArticlesPerPage) != 0) pagesCount++;
            if (page.HasValue)
            {
                if (page > pagesCount) page = pagesCount;
                currentPage = page.Value;
                int elementNo = page.Value * ArticlesPerPage - ArticlesPerPage;
                for (int i = 0; i < ArticlesPerPage; i++)
                {
                    if (elementNo < articlesList.Count && elementNo >= 0)
                    {
                        articlesListPaged.Add(articlesList.ElementAt(elementNo));
                    }
                    elementNo++;
                }
                foreach (var article in articlesList)
                {
                    var photo = dataBase.ArticlesPhotos.Where(a => a.ArticleId == article.ArticleId).First();
                    articlesPhotosPaged.Add(photo);
                }
                int j = 0;
                foreach (var article in articlesListPaged)
                {
                    articlesCommentsCount[j] = dataBase.ArticlesComments.Where(id => id.ArticleId == article.ArticleId).Count();
                    j++;
                }
            }
            ArticleDateListViewModel VM = new ArticleDateListViewModel { ArticlesList = articlesListPaged, ArticlesPhotosList = articlesPhotosPaged, AllPages = pagesCount, Page = currentPage, ArticlesCategoryList = dataBase.ArticlesCategories, Date = Date, ArticlesCommentsCount = articlesCommentsCount };
            return PartialView("_ArticlesListByDateShow", VM);
        }
        public ActionResult ArticleRateAdd(int ArticleId, int RateValue)
        {
            HttpCookie cookie = new HttpCookie("ArticleRate" + ArticleId.ToString(), RateValue.ToString());
            cookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Set(cookie);
            ArticlesRate rate = new ArticlesRate { ArticleId = ArticleId, Value = RateValue };
            dataBase.ArticlesRates.Add(rate);
            dataBase.SaveChanges();
            ArticleRateViewModel VM = new ArticleRateViewModel();
            VM.IsRated = true;
            VM.RateValue = int.Parse(cookie.Value);
            var allArticleRates = dataBase.ArticlesRates.Where(id => id.ArticleId == ArticleId);
            int allArticleRateSum = 0;
            if (allArticleRates.Count() > 0)
            {
                foreach (var articleRate in allArticleRates)
                {
                    allArticleRateSum = allArticleRateSum + articleRate.Value;
                }
                VM.RateAvarage = Convert.ToDouble(allArticleRateSum) / Convert.ToDouble(allArticleRates.Count());
            }
            VM.VotesCount = allArticleRates.Count();
            VM.ArticleId = ArticleId;

            return PartialView("_ArticleRateShow", VM);
        }
        public ActionResult ArticleRateShow(int ArticleId)
        {
            ArticleRateViewModel VM = new ArticleRateViewModel();
            if (Request.Cookies.Get("ArticleRate" + ArticleId.ToString()) != null)
            {
                var cookie = Request.Cookies.Get("ArticleRate" + ArticleId.ToString());
                VM.IsRated = true;
                VM.RateValue = int.Parse(cookie.Value);
                var allArticleRates = dataBase.ArticlesRates.Where(id => id.ArticleId == ArticleId);
                int allArticleRateSum = 0;
                if (allArticleRates.Count() > 0)
                {
                    foreach (var articleRate in allArticleRates)
                    {
                        allArticleRateSum = allArticleRateSum + articleRate.Value;
                    }
                    VM.RateAvarage = Convert.ToDouble(allArticleRateSum) / Convert.ToDouble(allArticleRates.Count());
                }
                VM.VotesCount = allArticleRates.Count();
                VM.ArticleId = ArticleId;
            }
            else
            {
                VM.IsRated = false;
                VM.ArticleId = ArticleId;
            }
            return PartialView("_ArticleRateShow", VM);
        }
        public ActionResult ArticleCommentsShow(int ArticleId)
        {
            var commentsList = dataBase.ArticlesComments.Where(id => id.ArticleId == ArticleId).Where(v => v.Visible == true).OrderBy(d => d.DateOfPublish).ToList();
            ArticleCommentsViewModel VM = new ArticleCommentsViewModel { ArticleId = ArticleId, ArticleCommentsList = commentsList };
            return PartialView("_ArticleCommentsShow", VM);
        }
        public async Task<ActionResult> ArticleCommentAdd(ArticlesComment comment)
        {
            var isCaptchaValid = await IsCaptchaValid(comment.GoogleRecaptchaV3);
            if (isCaptchaValid)
            {
                comment.DateOfPublish = DateTime.Now;
                comment.Visible = true;
                comment.Text = Server.HtmlEncode(comment.Text);
                dataBase.ArticlesComments.Add(comment);
                dataBase.SaveChanges();
                var commentsList = dataBase.ArticlesComments.Where(id => id.ArticleId == comment.ArticleId).Where(v => v.Visible == true).OrderBy(d => d.DateOfPublish).ToList();
                ArticleCommentsViewModel VM = new ArticleCommentsViewModel { ArticleId = comment.ArticleId, ArticleCommentsList = commentsList };
                return PartialView("_ArticleCommentsShow", VM);
            }
            else
            {
                string valError = "Zabezpieczenie antyspamowe uznało Cię za bota... Jeśli jesteś człowiekiem możesz skontaktować się z administratorem wysyłając email na adres: tech@simpleblog";
                var commentsList = dataBase.ArticlesComments.Where(id => id.ArticleId == comment.ArticleId).Where(v => v.Visible == true).OrderBy(d => d.DateOfPublish).ToList();
                ArticleCommentsViewModel VM = new ArticleCommentsViewModel { ArticleId = comment.ArticleId, ArticleCommentsList = commentsList, ValidationMsn = valError };
                return PartialView("_ArticleCommentsShow", VM);
            }
        }
        public ActionResult AricleCommentRatesShow(int ArticleCommentId, int? ArticleCommentRateValue)
        {
            ArticleCommentRateViewModel VM = new ArticleCommentRateViewModel();
            //jakas wartosc przekazana
            if (ArticleCommentRateValue.HasValue)
            {
                var rateValue = ArticleCommentRateValue.Value;
                //zmiana oceny
                if (Request.Cookies.Get("ArticleCommentRate" + ArticleCommentId.ToString()) != null)
                {
                    var cookie = Request.Cookies.Get("ArticleCommentRate" + ArticleCommentId.ToString());
                    var cookieReset = Request.Cookies.Get("ArticleCommentRate" + ArticleCommentId.ToString());
                    cookieReset.Expires = DateTime.Now.AddDays(-2);
                    Response.Cookies.Add(cookieReset);
                    cookie.Value = rateValue.ToString();
                    cookie.Expires = DateTime.Now.AddDays(365);
                    Response.Cookies.Add(cookie);

                    ArticlesCommentRate newRate = new ArticlesCommentRate { ArticlesCommentId = ArticleCommentId, Value = rateValue };
                    dataBase.ArticlesCommentsRates.Add(newRate);
                    ArticlesCommentRate oldRate = dataBase.ArticlesCommentsRates.Where(id => id.ArticlesCommentId == ArticleCommentId).Where(v => v.Value != rateValue).First();
                    dataBase.ArticlesCommentsRates.Remove(oldRate);
                    dataBase.SaveChanges();
                }
                //nowa ocena
                else
                {
                    HttpCookie cookie = new HttpCookie("ArticleCommentRate" + ArticleCommentId.ToString(), rateValue.ToString());
                    cookie.Expires = DateTime.Now.AddDays(365);
                    Response.Cookies.Set(cookie);
                    ArticlesCommentRate newRate = new ArticlesCommentRate { ArticlesCommentId = ArticleCommentId, Value = rateValue };
                    dataBase.ArticlesCommentsRates.Add(newRate);
                    dataBase.SaveChanges();
                }

                VM.ArticleCommentRateValue = rateValue;
            }
            else
            //wyswietlenie, bez wprowadzania zmian
            {
                if (Request.Cookies.Get("ArticleCommentRate" + ArticleCommentId.ToString()) != null)
                {
                    var cookie = Request.Cookies.Get("ArticleCommentRate" + ArticleCommentId.ToString());
                    VM.ArticleCommentRateValue = int.Parse(cookie.Value);
                }
                else
                {
                    VM.ArticleCommentRateValue = 0;
                }
            }
            VM.ArticleCommentId = ArticleCommentId;
            VM.ArticleCommentLikesCount = dataBase.ArticlesCommentsRates.Where(id => id.ArticlesCommentId == ArticleCommentId).Where(v => v.Value == 1).Count();
            VM.ArticleCommentDislikesCount = dataBase.ArticlesCommentsRates.Where(id => id.ArticlesCommentId == ArticleCommentId).Where(v => v.Value == -1).Count();
            return PartialView("_AricleCommentRatesShow", VM);
        }
        public class CaptchaResponseViewModel
        {
            public bool Success { get; set; }

            [JsonProperty(PropertyName = "error-codes")]
            public IEnumerable<string> ErrorCodes { get; set; }

            [JsonProperty(PropertyName = "challenge_ts")]
            public DateTime ChallengeTime { get; set; }

            public string HostName { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }
        }
        private async Task<bool> IsCaptchaValid(string response)
        {
            //try
            //{
            //    var secret = "secretKey";
            //    using (var client = new HttpClient())
            //    {
            //        var values = new Dictionary<string, string>
            //        {
            //            {"secret", secret},
            //            {"response", response},
            //            {"remoteip", Request.UserHostAddress}
            //        };

            //        var content = new FormUrlEncodedContent(values);
            //        var verify = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            //        var captchaResponseJson = await verify.Content.ReadAsStringAsync();
            //        var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(captchaResponseJson);
            //        return captchaResult.Success
            //               && captchaResult.Action == "articleComment"
            //               && captchaResult.Score > 0.5;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            return true;
        }
        public ActionResult SerchResultIndex(int? pageNumber, SearchBarViewModel VM)
        {
            int pageNo = 1;
            if (pageNumber.HasValue)
            {
                pageNo = pageNumber.Value;
                if (pageNo <= 1) pageNo = 1;
            }

            var VMSR = new SearchResultIndexViewModel { PageNumber = pageNo, SearchText = VM.searchText };
            return View(VMSR);
        }
        public ActionResult SerchResultIndexx(int? pageNumber, string searchText)
        {
            int pageNo = 1;
            if (pageNumber.HasValue)
            {
                pageNo = pageNumber.Value;
                if (pageNo <= 1) pageNo = 1;
            }

            var VMSR = new SearchResultIndexViewModel { PageNumber = pageNo, SearchText = searchText };
            return View("SerchResultIndex", VMSR);
        }
        public ActionResult SearchResultList(int? page, string searchText)
        {
            var allArticles = dataBase.Articles.Where(v => v.Visible).Where(d => d.DateOfPublish < DateTime.Now).OrderByDescending(d => d.DateOfPublish);
            var articlesList = new List<Article>();
            foreach (var article in allArticles.ToList())
            {
                var titleLow = article.Title.ToLower();
                var searchTextLow = searchText.ToLower();
                if (titleLow.Contains(searchTextLow))
                {
                    articlesList.Add(article);
                }
            }

            int ArticlesPerPage = 10;
            var articlesListPaged = new List<Article>();
            var articlesPhotosPaged = new List<ArticlesPhoto>();
            int[] articlesCommentsCount = new int[ArticlesPerPage];
            int currentPage = 1;
            int pagesCount = articlesList.Count / ArticlesPerPage;
            if ((articlesList.Count % ArticlesPerPage) != 0) pagesCount++;
            if (page.HasValue)
            {
                if (page > pagesCount) page = pagesCount;
                currentPage = page.Value;
                int elementNo = page.Value * ArticlesPerPage - ArticlesPerPage;
                for (int i = 0; i < ArticlesPerPage; i++)
                {
                    if (elementNo < articlesList.Count && elementNo >= 0)
                    {
                        articlesListPaged.Add(articlesList.ElementAt(elementNo));
                    }
                    elementNo++;
                }
                foreach (var article in articlesList)
                {
                    var photo = dataBase.ArticlesPhotos.Where(a => a.ArticleId == article.ArticleId).First();
                    articlesPhotosPaged.Add(photo);
                }
                int j = 0;
                foreach (var article in articlesListPaged)
                {
                    articlesCommentsCount[j] = dataBase.ArticlesComments.Where(id => id.ArticleId == article.ArticleId).Count();
                    j++;
                }
            }

            var sbvm = new SearchBarViewModel { searchText = searchText };
            ArticleCategoryListViewModel VM = new ArticleCategoryListViewModel { ArticlesList = articlesListPaged, ArticlesPhotosList = articlesPhotosPaged, AllPages = pagesCount, Page = currentPage, ArticlesCategoryList = dataBase.ArticlesCategories, ArticlesCommentsCount = articlesCommentsCount, SBVM = sbvm};
            return PartialView("_SearchResultList", VM);
        }
        public ActionResult SearchBarShow()
        {
            return PartialView("_SearchBarShow");
        }
        public ActionResult CookiesPolicyShow()
        {
            return View();
        }
        public ActionResult PrivacyPolicyShow()
        {
            return View();
        }
        public ActionResult TermsOfUseShow()
        {
            return View();
        }
        public ActionResult ContactShow()
        {
            var VM = new ContactViewModel();
            return View("ContactShow", VM);
        }
        public ActionResult ContactShoww(string msn)
        {
            var VM = new ContactViewModel { ValidationMsn = msn };
            return View("ContactShow", VM);
        }
        public ActionResult ContactFormShow()
        {
            return PartialView("_ContactFormShow");
        }
        [HttpPost]
        public ActionResult ContactFormShow(ContactFormViewModel VM)
        {
            if (ModelState.IsValid)
            {
                SmtpClient client = new SmtpClient("host");
                client.Credentials = new NetworkCredential("info@simpleblog", "password");
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("info@simpleblog");
                mailMessage.Subject = "Wiadomość z simpleblog";
                mailMessage.Body = "Imię: " + VM.Name + "\nNr telefonu: " + VM.Phone + "\nE-mail: " + VM.Email + "\nTreść wiadomości: " + VM.MessageText;
                client.Send(mailMessage);

                var msn1 = "Wiadomość została wysłana! Dziękujemy.";
                return RedirectToAction("ContactShoww", "Home", new { msn = msn1 });
            }
            else
            {
                var msn1 = "Wiadomość nie została wysłana, spróbuj później...";
                return RedirectToAction("ContactShoww", "Home", new { msn = msn1 });
            }
        }
        public ActionResult SetCookiesWarning()
        {
            HttpCookie cookie = new HttpCookie("simpleblog-cookiesAcceptation", "true");
            cookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Set(cookie);
            return RedirectToAction("ShowCookiesWarning", "Home");
        }
        public ActionResult ShowCookiesWarning()
        {
            return PartialView("_ShowCookiesWarning");
        }
    }
}