using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;

namespace ShoppingListApp.Web.UI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IItemsRepository>().To<ItemXmlRepository>();
            kernel.Bind<IShoppingListRepository>().To<ShoppingListXmlRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ItemXmlRepositoryName>().WhenInjectedInto<IItemsRepository>();  
            kernel.Bind<IRepositoryNameProvider>().To<ShoppingListXmlRepositoryName>().WhenInjectedInto<IShoppingListRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ItemXmlRepositoryName>().Named("ItemRepositoryName");
            kernel.Bind<IRepositoryNameProvider>().To<ShoppingListXmlRepositoryName>().Named("ShoppingListRepositoryName");
            kernel.Bind<IUserInformation>().To<GoogleUserInformation>();
            kernel.Bind<IBackupProcessor>().To<EmailBackupProcessor>().WithConstructorArgument("settings", new GoogleEmailSettings(new GoogleUserInformation()));   
        }
    }
}