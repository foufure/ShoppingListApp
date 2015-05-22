using Ninject;
using Quartz;
using Quartz.Spi;

namespace ShoppingListApp.JobsScheduler
{
    public class NinjectJobFactory : IJobFactory
    {
        private IKernel kernel;

        public NinjectJobFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }
        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)kernel.Get<IJob>(bundle.JobDetail.JobType.Name);
        }

        public void ReturnJob(IJob job)
        {
            this.kernel.Release(job);
        }
    }
}
