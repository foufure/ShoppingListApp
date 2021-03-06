// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Scope = "type", Target = "ShoppingListApp.JobsScheduler.CronJobsScheduler", Justification = "Reviewed. Cron is allowed. False-Positive.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Scope = "member", Target = "ShoppingListApp.JobsScheduler.CronJobsScheduler.#InitializeJobScheduler(System.String)", Justification = "Reviewed. Cron is allowed. False-Positive.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ninject", Scope = "type", Target = "ShoppingListApp.JobsScheduler.NinjectJobFactory", Justification = "Reviewed. Ninject is allowed. False-Positive.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member", Target = "ShoppingListApp.JobsScheduler.BackupAllJob.#.ctor(ShoppingListApp.Domain.Abstract.IBackupProcessor,ShoppingListApp.Domain.Abstract.IDataPathProvider)", Justification = "Reviewed. The object is provided by dependency injection. No validation required.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member", Target = "ShoppingListApp.JobsScheduler.BackupAllJob.#.ctor(ShoppingListApp.Domain.Abstract.IBackupProcessor,ShoppingListApp.Domain.Abstract.IDataPathProvider,NLog.Interface.ILogger)", Justification = "Reviewed. The object is provided by dependency injection. No validation required.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "ShoppingListApp.JobsScheduler.CronJobsScheduler.#.ctor(Quartz.ISchedulerFactory,Quartz.Spi.IJobFactory)", Justification = "Reviewed. The object is provided by dependency injection. No validation required.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Scope = "member", Target = "ShoppingListApp.JobsScheduler.CronJobsScheduler.#AddJob(System.String,Quartz.IJobDetail)", Justification = "Reviewed. Cron is allowed. False-Positive.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ToRun", Scope = "member", Target = "ShoppingListApp.JobsScheduler.CronJobsScheduler.#AddJob(System.String,Quartz.IJobDetail)", Justification = "Reviewed. ToRun is allowed. False-Positive.")]
