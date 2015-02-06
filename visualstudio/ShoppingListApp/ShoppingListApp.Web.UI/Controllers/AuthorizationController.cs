using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Web.UI.Infrastructure.Abstract;
using ShoppingListApp.Web.UI.Models;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class AuthorizationController : Controller
    {
        IAuthenticationProvider authProvider;

        public AuthorizationController(IAuthenticationProvider authProviderParam)
        {
            authProvider = authProviderParam;
        }

        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.UserName, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View();
                }
            }
            else 
            {
                return View();
            }
        }
    }
}