using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;
using Quartz.Impl;

namespace ShoppingListApp.JobsScheduler
{
    public static class CronJobsScheduler
    {
        private static Logger cronStartLogger = LogManager.GetCurrentClassLogger();

        public static void InitializeJobScheduler(string userCronSchedule)
        {
            // http://blog.appharbor.com/2012/4/18/scheduled-tasks-using-quartz-and-appharbor-background-workers
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<BackupAllJob>().Build();

            // http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger
            string cronSchedule = @"0 5 8,10,12,18,20 * * ?"; // Every day at 8:05, 10:05 ... - ("0 * * ? * *") - Every minute

            if (!string.IsNullOrEmpty(userCronSchedule))
            { 
                cronSchedule = userCronSchedule;
            }
            
            var cron = TriggerBuilder.Create()
                            .WithCronSchedule("0 * * ? * *")
                            .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second)) // Next scheduled backup is configured ASAP after the service is up
                            .Build();

            scheduler.ScheduleJob(job, cron);

            cronStartLogger.Trace("Cron Scheduler Started: " + DateTime.Now.ToString());
        }
    }
}
