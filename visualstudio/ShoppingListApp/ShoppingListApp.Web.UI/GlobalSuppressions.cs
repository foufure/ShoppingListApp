// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "ShoppingListApp.Web.UI.Controllers.AccountController.#ExternalLogOnCallback(System.String)", Justification = "Reviewed. ActionMethod cannot be static. It does not work.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "ShoppingListApp.Web.UI.MvcApplication.#Application_Start()", Justification = "Reviewed. Application_Start specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "ShoppingListApp.Web.UI.Startup.#ConfigureAuth(Owin.IAppBuilder)", Justification = "Reviewed. Owin specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ninject", Scope = "type", Target = "ShoppingListApp.Web.UI.App_Start.NinjectWebCommon", Justification = "Reviewed. Ninject specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Scope = "type", Target = "ShoppingListApp.Web.UI.BundleConfig", Justification = "Reviewed. BundleConfig specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "ShoppingListApp.Web.UI.BundleConfig.#RegisterBundles(System.Web.Optimization.BundleCollection)", Justification = "Reviewed. BundleConfig specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Scope = "type", Target = "ShoppingListApp.Web.UI.RouteConfig", Justification = "Reviewed. RouteConfig specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "ShoppingListApp.Web.UI.Controllers.ShoppingListController.#ShowShoppingList(System.Nullable`1<System.UInt32>)", Justification = "Reviewed. Default parameters are common practice in ASP.NET MVC")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ninject", Scope = "type", Target = "ShoppingListApp.Web.UI.Infrastructure.NinjectDependencyResolver", Justification = "Reviewed. Ninject specific.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Scope = "member", Target = "ShoppingListApp.Web.UI.Controllers.AccountController.#LogOn(System.String)", Justification = "Reviewed. Google Account Controller.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Scope = "member", Target = "ShoppingListApp.Web.UI.Controllers.AccountController.#ExternalLogOnCallback(System.String)", Justification = "Reviewed. Google Account Controller.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Scope = "member", Target = "ShoppingListApp.Web.UI.Controllers.UserCultureController.#GetUserCulture(System.String,System.String)", Justification = "Reviewed. Url to the same page.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "ShoppingListApp.Web.UI.Controllers.HomeController.#RestoreAllBackups(System.Web.HttpPostedFileBase)", Justification = "Reviewed. DOTNETZIP throws a general exception in this case.")]
