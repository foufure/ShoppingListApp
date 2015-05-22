using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ionic.Zip;
using NLog;
using NLog.Interface;
using Quartz;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.JobsScheduler
{
    public class DummyPingJob : IJob
    {
        private ILogger dummyPingJobLogger;

        public DummyPingJob(ILogger dummyPingJobLogger)
        {
            this.dummyPingJobLogger = dummyPingJobLogger;
        }

        public void Execute(IJobExecutionContext context)
        {
            // This is done to keep IIS alive, because after 20 minutes it shuts down on AppHarbor.
            using (WebClient client = new WebClient())
            {
                client.DownloadString("http://shoppinglistapp.apphb.com");
            }
        }
    }
}
