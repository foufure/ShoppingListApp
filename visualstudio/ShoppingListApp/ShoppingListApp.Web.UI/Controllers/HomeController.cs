using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;
using ShoppingListApp.i18n.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private IBackupProcessor backupProcessor;
        private IUserInformation userInformation;

        public HomeController(IBackupProcessor backupProcessorParam, IUserInformation userInformationParam)
        {
            backupProcessor = backupProcessorParam;
            userInformation = userInformationParam;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult RefreshAfterLogin()
        {
            return View("Index");
        }

        [Authorize]
        public RedirectToRouteResult Backup()
        {
            backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ArticleRepository." + userInformation.UserName + @".xml");
            backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + ".xml");
            TempData["backup"] = ShoppingListApp.i18n.Resources.Views.Home.IndexCommon.BackupMessage + " " + DateTime.Now.ToString("d", CultureHelper.getCurrentUICulture());
            return RedirectToAction("Index");
        }
    }
}