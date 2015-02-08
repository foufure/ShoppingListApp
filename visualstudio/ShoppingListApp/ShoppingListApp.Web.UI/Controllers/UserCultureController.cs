using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.i18n.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class UserCultureController : Controller
    {
        public ActionResult GetUserCulture(string userCulture, string returnUrl)
        {
            CultureHelper cultureHelper = new CultureHelper();
            cultureHelper.SetWantedUserCulture(this.ControllerContext.HttpContext.Response.Cookies, userCulture);
            return Redirect(returnUrl);
        }
    }
}