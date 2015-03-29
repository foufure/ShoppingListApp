using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using NLog;

namespace ShoppingListApp.JobsScheduler
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Justification = "Reviewed")]
    public static class CronJobsScheduler
    {
        private static Logger cronStartLogger = LogManager.GetCurrentClassLogger();

        public static void InitializeJobScheduler()
        {
            cronStartLogger.Trace("Start Initialization of Cron: " + DateTime.Now.ToString());

            // http://blog.appharbor.com/2012/4/18/scheduled-tasks-using-quartz-and-appharbor-background-workers
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            cronStartLogger.Trace("Cron Scheduler Initialized: " + DateTime.Now.ToString());

            var job = JobBuilder.Create<BackupAllJob>().Build();

            cronStartLogger.Trace("Cron Job Created: " + DateTime.Now.ToString());

            // http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger
            var cron = TriggerBuilder.Create()
                            //// .WithCronSchedule("0 * * ? * *") // Every minute
                            .WithCronSchedule("0 5 8,10,12,18,20 * * ?") // Every day at 8:05, 10:05 ...
                            //// .WithCronSchedule("0 0 23 ? * *") // Every day at 23:00
                            .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second)) // Next scheduled backup is configured ASAP after the service is up
                            .Build();

            cronStartLogger.Trace("Cron Trigger Created: " + DateTime.Now.ToString());

            scheduler.ScheduleJob(job, cron);

            cronStartLogger.Trace("Cron Scheduler Started: " + DateTime.Now.ToString());
        }
    }
}
