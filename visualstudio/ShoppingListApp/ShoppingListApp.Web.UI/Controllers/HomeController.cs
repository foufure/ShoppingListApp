using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private IBackupProcessor backupProcessor;

        public HomeController(IBackupProcessor backupProcessorParam)
        {
            backupProcessor = backupProcessorParam;
        }

        public ActionResult Index()
        {
            return View();
        }

        public RedirectToRouteResult Backup()
        {
            backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ArticleRepository.xml");
            backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository.xml");
            TempData["backup"] = "Backup Done on "+ DateTime.Now;
            return RedirectToAction("Index");
        }
    }
}