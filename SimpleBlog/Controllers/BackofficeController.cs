using SimpleBlog.Classes;
using SimpleBlog.DataAccessLayer;
using SimpleBlog.Infrastructure;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Controllers
{
    public class BackofficeController : Controller
    {
        private SimpleBlogContext db = new SimpleBlogContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        // GET: Backoffice
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ArticlesManage()
        {
            var VM = new ArticlesManageViewModel { ArticlesList = db.Articles.OrderByDescending(d => d.DateOfPublish) };
            return View(VM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ArticleAdd(int? articleId, bool? confirmation)
        {
            Article article;
            if (articleId.HasValue)
            {
                ViewBag.EditMode = true;
                article = db.Articles.Find(articleId);
                article.Text = Server.HtmlDecode(article.Text);
                article.IntroText = Server.HtmlDecode(article.IntroText);
            }
            else
            {
                ViewBag.EditMode = false;
                article = new Article();
                article.DateOfPublish = DateTime.Now;
            }
            var tags = db.ArticlesTags.OrderBy(tn => tn.Name).ToList();
            List<TagCheck> tagsCheck = new List<TagCheck>();
            foreach (var tag in tags)
            {
                var tagCheck = new TagCheck { Tag = tag, Value = false };
                tagsCheck.Add(tagCheck);
            }
            var vm = new ArticleAddViewModel() { Article = article, ArticlesCategories = db.ArticlesCategories, TagsList = db.ArticlesTags.OrderBy(t => t.Name).ToList(), TagsListChecked = tagsCheck };
            vm.Confirmation = confirmation;
            return View("ArticleAdd", vm);
        }
        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Admin")]
        public ActionResult ArticleAdd(ArticleAddViewModel vm, List<HttpPostedFileBase> files, List<TagCheck> TagsListChecked)
        {
            if (vm.Article.ArticleId > 0)
            {
                //edycja newsa
                vm.Article.DateOfPublish = DateTime.Parse(Request.Form["DateOfPublish"]);
                vm.Article.AuthorId = User.Identity.GetUserId();
                vm.Article.Text = Server.HtmlEncode(vm.Article.Text);

                string tagLine = "";
                if (vm.TagsListChecked != null)
                {
                    foreach (var tagChecked in vm.TagsListChecked)
                    {
                        if (tagChecked.Value == true)
                        {
                            if (tagLine == "")
                            {
                                tagLine = tagChecked.Tag.Name;
                            }
                            else
                            {
                                tagLine = tagLine + "," + tagChecked.Tag.Name;
                            }
                        }
                    }
                }
                if (vm.Tags != null)
                {
                    vm.Tags = vm.Tags + "," + tagLine;
                }
                else
                {
                    vm.Tags = tagLine;
                }

                if (vm.Tags != null)
                {
                    var tags = vm.Tags.Split(',');
                    foreach (var tag in tags)
                    {
                        if (db.ArticlesTags.Where(n => n.Name == tag).Count() == 0)
                        {
                            db.ArticlesTags.Add(new ArticlesTag { Name = tag });
                            db.SaveChanges();
                        }
                        ArticlesTag artTag = db.ArticlesTags.Where(n => n.Name == tag).First();
                        if (artTag.Name != "" || artTag.Name != " ")
                        {
                            db.ArticlesTagsConnections.Add(new ArticlesTagsConnection { ArticleId = vm.Article.ArticleId, ArticlesTagId = artTag.ArticlesTagId });
                            db.SaveChanges();
                        }
                    }
                }
                db.Entry(vm.Article).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ArticleAdd", new { confirmation = true });
            }
            else
            {
                //dodawanie nowego newsa
                if (files != null && files.Count > 0) // != &&
                {
                    if (vm.Article.Title == null)
                    {
                        ModelState.AddModelError("", "Wpisz TYTUŁ.");
                    }
                    if (vm.Article.Text == null)
                    {
                        ModelState.AddModelError("", "Wpisz jakiś TEKST.");
                    }
                    if (vm.Article.Title == null || vm.Article.Text == null)
                    {
                        var articlesCategories = db.ArticlesCategories.ToList();
                        vm.ArticlesCategories = articlesCategories;
                        return View(vm);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            //zapisywanie artykułu
                            vm.Article.DateOfPublish = DateTime.Parse(Request.Form["DateOfPublish"]);
                            vm.Article.Visible = vm.Article.Visible;
                            vm.Article.AuthorId = User.Identity.GetUserId();
                            vm.Article.Text = Server.HtmlEncode(vm.Article.Text);
                            vm.Article.IntroText = Server.HtmlEncode(vm.Article.IntroText);
                            db.Entry(vm.Article).State = System.Data.Entity.EntityState.Added;
                            db.SaveChanges();
                            //////////////////////////////////

                            //zapisywanie tagów
                            string tagLine = "";
                            if (vm.TagsListChecked != null)
                            {
                                foreach (var tagChecked in vm.TagsListChecked)
                                {
                                    if (tagChecked.Value == true)
                                    {
                                        if (tagLine == "")
                                        {
                                            tagLine = tagChecked.Tag.Name;
                                        }
                                        else
                                        {
                                            tagLine = tagLine + "," + tagChecked.Tag.Name;
                                        }
                                    }
                                }
                            }
                            if (vm.Tags != null)
                            {
                                vm.Tags = vm.Tags + "," + tagLine;
                            }
                            else
                            {
                                vm.Tags = tagLine;
                            }

                            if (vm.Tags != null)
                            {
                                var tags = vm.Tags.Split(',');
                                foreach (var tag in tags)
                                {
                                    if (tag.Length > 0)
                                    {
                                        if (db.ArticlesTags.Where(n => n.Name == tag).Count() == 0)
                                        {
                                            db.ArticlesTags.Add(new ArticlesTag { Name = tag });
                                            db.SaveChanges();
                                        }
                                        ArticlesTag artTag = db.ArticlesTags.Where(n => n.Name == tag).First();
                                        db.ArticlesTagsConnections.Add(new ArticlesTagsConnection { ArticleId = vm.Article.ArticleId, ArticlesTagId = artTag.ArticlesTagId });
                                        db.SaveChanges();
                                    }
                                }
                            }
                            /////////////////////////////

                            //zapisywanie zdjec
                            var photo = new ArticlesPhoto();
                            int i = 0;
                            foreach (var file in files)
                            {
                                var fileExt = Path.GetExtension(file.FileName);
                                var filename = Guid.NewGuid() + fileExt;
                                var path = Path.Combine(Server.MapPath(AppConfig.ArticlesImagesFolderRelated), filename);
                                file.SaveAs(path);

                                photo.ArticleId = vm.Article.ArticleId;
                                photo.FilePath = filename;
                                photo.OrderNo = i;
                                photo.Width = 1300;
                                photo.Height = 722;
                                db.ArticlesPhotos.Add(photo);
                                db.SaveChanges();
                                i++;
                            }
                            /////////////////////////////////////////////////////////////////////////////////////////

                            return RedirectToAction("ArticleAdd", new { confirmation = true });
                        }
                        else
                        {
                            var articleCategories = db.ArticlesCategories.ToList();
                            vm.ArticlesCategories = articleCategories;
                            return View(vm);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nie wskazano pliku.");
                    var articleCategories = db.ArticlesCategories.ToList();
                    vm.ArticlesCategories = articleCategories;
                    return View("ArticleAdd", vm);
                }
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ArticlesCategoriesManage()
        {
            var articlesCategories = db.ArticlesCategories;
            var VM = new ArticlesCategoriesManageViewModel { ArticleCategories = articlesCategories };
            return View(VM);
        }
        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Admin")]
        public ActionResult ArticlesCategoriesManage(string ArticlesCategoryAddName)
        {
            var newArticleCategory = new ArticlesCategory { Name = ArticlesCategoryAddName };
            db.ArticlesCategories.Add(newArticleCategory);
            db.SaveChanges();
            return RedirectToAction("ArticlesCategoriesManage", "Backoffice");
            //var articlesCategories = db.ArticlesCategories;
            //var VM = new ArticlesCategoriesManageViewModel { ArticleCategories = articlesCategories};
            //return View(VM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ArticlesCategoriesDelete(string ArticleCategoryName)
        {
            var articlesCategoryToRemove = db.ArticlesCategories.Where(n => n.Name == ArticleCategoryName).First();
            db.ArticlesCategories.Remove(articlesCategoryToRemove);
            db.SaveChanges();
            var articlesCategories = db.ArticlesCategories;
            var VM = new ArticlesCategoriesManageViewModel { ArticleCategories = articlesCategories };
            return View("ArticlesCategoriesManage", VM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ArticleDelete(int ArticleId)
        {
            bool Barticle = false, Bcomments = false, BcommentsRates = false, Bphotos = false, Brates = false, BtagsConnections = false;

            var article = db.Articles.Where(aid => aid.ArticleId == ArticleId).First();
            db.Articles.Remove(article);
            db.SaveChanges();
            if (db.Articles.Where(aid => aid.ArticleId == ArticleId).Count() == 0) Barticle = true;

            var articleComments = db.ArticlesComments.Where(aid => aid.ArticleId == ArticleId).ToList();
            foreach (var comment in articleComments)
            {
                db.ArticlesComments.Remove(comment);
                db.SaveChanges();
            }
            if (db.ArticlesComments.Where(aid => aid.ArticleId == ArticleId).Count() == 0) Bcomments = true;

            var articleCommentsRates = db.ArticlesCommentsRates.Where(aid => aid.ArticleId == ArticleId).ToList();
            foreach (var rate in articleCommentsRates)
            {
                db.ArticlesCommentsRates.Remove(rate);
                db.SaveChanges();
            }
            if (db.ArticlesCommentsRates.Where(aid => aid.ArticleId == ArticleId).Count() == 0) BcommentsRates = true;

            var articlePhotos = db.ArticlesPhotos.Where(aid => aid.ArticleId == ArticleId).ToList();
            foreach (var photo in articlePhotos)
            {
                var path = Path.Combine(Server.MapPath(AppConfig.ArticlesImagesFolderRelated), photo.FilePath);
                if (System.IO.File.Exists(path) == true)
                {
                    System.IO.File.Delete(path);
                }
                db.ArticlesPhotos.Remove(photo);
                db.SaveChanges();
            }
            if (db.ArticlesPhotos.Where(aid => aid.ArticleId == ArticleId).Count() == 0) Bphotos = true;

            var articleRates = db.ArticlesRates.Where(aid => aid.ArticleId == ArticleId).ToList();
            foreach (var rate in articleRates)
            {
                db.ArticlesRates.Remove(rate);
                db.SaveChanges();
            }
            if (db.ArticlesRates.Where(aid => aid.ArticleId == ArticleId).Count() == 0) Brates = true;

            var articleTagsConnections = db.ArticlesTagsConnections.Where(aid => aid.ArticleId == ArticleId).ToList();
            foreach (var connection in articleTagsConnections)
            {
                db.ArticlesTagsConnections.Remove(connection);
                db.SaveChanges();
            }
            if (db.ArticlesTagsConnections.Where(aid => aid.ArticleId == ArticleId).Count() == 0) BtagsConnections = true;

            ArticleDeleteViewModel VM = new ArticleDeleteViewModel { Success = false };
            if (Barticle && Bcomments && BcommentsRates && Bphotos && Brates && BtagsConnections)
            {
                VM = new ArticleDeleteViewModel { ArticleId = ArticleId, Comments = articleComments.Count(), CommentsRates = articleCommentsRates.Count(), Photos = articlePhotos.Count(), Rates = articleRates.Count(), TagConnections = articleTagsConnections.Count(), Success = true };
            }
            return PartialView(VM);
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public string CKEditorImageUpload()
        {
            return "Success";
        }

        [Authorize(Roles ="Admin")]
        public ActionResult TagsManageShow()
        {
            var tagsList = db.ArticlesTags.OrderBy(tn => tn.Name).ToList();
            var tagsConnectionList = db.ArticlesTagsConnections.ToList();
            var VM = new TagsManageShowViewModel { TagsList = tagsList, TagsConnectionsList = tagsConnectionList };
            return View(VM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult TagDelete(int TagId)
        {
            var tagToRemove = db.ArticlesTags.Where(tid => tid.ArticlesTagId == TagId).First();
            db.ArticlesTags.Remove(tagToRemove);
            db.SaveChanges();

            var tagConnectionsToRemove = db.ArticlesTagsConnections.Where(tid => tid.ArticlesTagId == TagId).ToList();
            db.ArticlesTagsConnections.RemoveRange(tagConnectionsToRemove);
            db.SaveChanges();

            return RedirectToAction("TagsManageShow", "Backoffice");
        }

    }
}