using System.Web.Mvc;
using ShoppingListApp.I18N.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    [AllowAnonymous]
    public class UserCultureController : Controller
    {
        public ActionResult SaveCultureChosenByUser(string userCulture, string returnUrl)
        {
            CultureChoiceUtils.SaveSupportedCultureChosenByUserInACookie(this.ControllerContext.HttpContext.Response.Cookies, userCulture);
            return Redirect(returnUrl);
        }
    }
}