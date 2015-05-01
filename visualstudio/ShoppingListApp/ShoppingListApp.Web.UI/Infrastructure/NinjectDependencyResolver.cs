using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using NLog.Interface;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.JobsScheduler;

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
            kernel.Bind<IDataPathProvider>().To<RuntimeDataPathProvider>().WhenInjectedInto<IJob>();
            kernel.Bind<IDataPathProvider>().To<ServerDataPathProvider>();
            kernel.Bind<IItemsRepository>().To<ItemXmlRepository>();
            kernel.Bind<IShoppingListRepository>().To<ShoppingListXmlRepository>();
            kernel.Bind<ICategoryRepository>().To<CategoryXmlRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ItemXmlRepositoryName>().WhenInjectedInto<IItemsRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<CategoryXmlRepositoryName>().WhenInjectedInto<ICategoryRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ShoppingListXmlRepositoryName>().WhenInjectedInto<IShoppingListRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ItemXmlRepositoryName>().Named("ItemRepositoryName");
            kernel.Bind<IRepositoryNameProvider>().To<CategoryXmlRepositoryName>().Named("CategoryRepositoryName");
            kernel.Bind<IRepositoryNameProvider>().To<ShoppingListXmlRepositoryName>().Named("ShoppingListRepositoryName");
            kernel.Bind<IUserInformation>().To<GoogleUserInformation>();
            kernel.Bind<IEmailSettings>().To<GoogleEmailSettings>();
            kernel.Bind<IBackupProcessor>().To<EmailBackupProcessor>();
            kernel.Bind<IJob>().To<BackupAllJob>();
            kernel.Bind<ILogger>().To<LoggerAdapter>().WithConstructorArgument(NLog.LogManager.GetCurrentClassLogger());
            kernel.Bind<ISchedulerFactory>().To<StdSchedulerFactory>();
            kernel.Bind<IJobFactory>().To<NinjectJobFactory>().WithConstructorArgument(kernel);
        }
    }
}