using Quartz;
using Quartz.Spi;

namespace ShoppingListApp.JobsScheduler
{
    public class CronJobsScheduler
    {
        private IScheduler scheduler;

        public CronJobsScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
        {
            schedulerFactory.GetScheduler().JobFactory = jobFactory;
            this.scheduler = schedulerFactory.GetScheduler();
        }

        public void AddJob(string userCronSchedule, IJobDetail jobToRun)
        {
            // http://quartz-scheduler.org/documentation/quartz-2.x/tutorials/
            ITrigger cron = TriggerBuilder.Create()
                            .WithCronSchedule(userCronSchedule)
                            .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second)) // Next scheduled backup is configured ASAP after the service is up
                            .Build();

            scheduler.ScheduleJob(jobToRun, cron);
        }

        public void StartJobScheduler()
        {
            scheduler.Start();
        }

        public void StandbyJobScheduler()
        {
            scheduler.Standby();
        }
    }
}
