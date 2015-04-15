using Ninject;
using Quartz;
using Quartz.Impl;

namespace ShoppingListApp.JobsScheduler
{
    public class CronJobsScheduler
    {
        private IKernel kernel;

        public CronJobsScheduler(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public void InitializeJobScheduler(string userCronSchedule)
        {
            // http://blog.appharbor.com/2012/4/18/scheduled-tasks-using-quartz-and-appharbor-background-workers
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.JobFactory = new NinjectJobFactory(kernel);
            scheduler.Start();

            var job = JobBuilder.Create<BackupAllJob>().Build();

            // http://quartz-scheduler.org/documentation/quartz-2.x/tutorials/
            string cronSchedule = @"0 5 8,10,12,18,20 * * ?"; // Every day at 8:05, 10:05 ... - ("0 * * ? * *") - Every minute

            if (!string.IsNullOrEmpty(userCronSchedule))
            { 
                cronSchedule = userCronSchedule;
            }
            
            var cron = TriggerBuilder.Create()
                            .WithCronSchedule(cronSchedule)
                            .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second)) // Next scheduled backup is configured ASAP after the service is up
                            .Build();

            scheduler.ScheduleJob(job, cron);
        }
    }
}
