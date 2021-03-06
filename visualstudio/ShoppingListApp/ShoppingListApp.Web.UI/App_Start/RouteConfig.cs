﻿using System.Web.Mvc;
using System.Web.Routing;

namespace ShoppingListApp.Web.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Categories",
                url: "{controller}/{action}/Category{categoryToModify}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "Items",
                url: "{controller}/{action}/Item{itemToModifyId}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "ShoppingLists",
                url: "{controller}/{action}/ShoppingList{shoppinglistId}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
