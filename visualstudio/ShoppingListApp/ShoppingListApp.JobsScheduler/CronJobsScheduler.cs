using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace ShoppingListApp.JobsScheduler
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Justification = "Reviewed")]
    public static class CronJobsScheduler
    {
        public static void InitializeJobScheduler()
        {
            // http://blog.appharbor.com/2012/4/18/scheduled-tasks-using-quartz-and-appharbor-background-workers
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<BackupAllJob>().Build();

            // http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger
            var cron = TriggerBuilder.Create()
                            //// .WithCronSchedule("0 * * ? * *") // Every minute
                            .WithCronSchedule("0 0 23 ? * *") // Every day at 23:00
                            .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second)) // Next scheduled backup is configured ASAP after the service is up
                            .Build();

            scheduler.ScheduleJob(job, cron);
        }
    }
}
