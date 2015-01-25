using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleRepository articleRepository;

        public ArticleController(IArticleRepository repositoryParam)
        {
            articleRepository = repositoryParam;
        }

        public ActionResult Articles()
        {
            return View(articleRepository.repository);
        }

        public RedirectToRouteResult AddNewArticle(string newArticleName)
        {
            if(newArticleName != "")
            { 
                articleRepository.Add(newArticleName);
                articleRepository.Save();
            }

            return RedirectToAction("Articles");
        }

        public RedirectToRouteResult RemoveArticle(uint articleToRemoveID)
        {
            articleRepository.Remove(articleToRemoveID);
            articleRepository.Save();
            return RedirectToAction("Articles");
        }

        [HttpGet]
        public ViewResult ModifyArticle(uint articleToModifyID)
        {
            return View(articleRepository.repository.Where(article => article.ArticleID == articleToModifyID).FirstOrDefault());
        }

        [HttpPost]
        public RedirectToRouteResult ModifyArticle(uint articleToModifyID, string articleToModifyNewName)
        {
            if (articleToModifyNewName != "")
            {
                articleRepository.Modify(new Article() { ArticleID = articleToModifyID, ArticleName = articleToModifyNewName });
                articleRepository.Save();
            }
            
            return RedirectToAction("Articles");
        } 
    }
}