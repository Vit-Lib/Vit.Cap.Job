using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Vit.Cap.Job.Entity;

namespace Vit.Cap.Job
{
    public static class JobHelp
    {

        public const string FailedThresholdMessageName = "Vit.Cap.Job.Failed";

        public static string GetCapEventName(string jobName, string jobEvent = JobEvent.JobWaitForStart, string jobGroup = null)
        {
            if (string.IsNullOrWhiteSpace(jobGroup))
            {
                jobGroup = DefaultJobGroup;
            }
            return $"Vit.Cap.Job:{jobGroup}:{jobName}:{jobEvent}";
        }


        public static string GetCapEventName(Entity.JobInfo jobInfo, string jobEvent= JobEvent.JobWaitForStart)
        {
            return GetCapEventName(jobInfo.jobName, jobEvent, jobInfo.jobGroup);
        }



        internal static string DefaultJobGroup = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Cap.VitCapJob.JobGroup")?? "DefaultJobGroup";

        internal static string DefaultCapGroup = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Cap.Cap.Group");

        internal static IApplicationBuilder app;

        public static IServiceScope CreateServiceScope()
        {
            return app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
        }


        /// <summary>
        /// JobHelp.CreateServiceScope((serviceProvider, capBus)=>{ });
        /// </summary>
        /// <param name="action"></param>
        public static void CreateServiceScope(Action<IServiceProvider, ICapPublisher>  action)
        {
            using var serviceScope = JobHelp.CreateServiceScope(); 
            DotNetCore.CAP.ICapPublisher _capBus = serviceScope.ServiceProvider.GetRequiredService<DotNetCore.CAP.ICapPublisher>();
            action(serviceScope.ServiceProvider, _capBus);
        }



        #region Publish JobStatus       

        public static List<JobInfo> GetJob(string ref_id)
        {
            using var serviceScope = JobHelp.CreateServiceScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();

            var result = (from job in dbContext.JobInfo
                          where job.ref_id == ref_id
                          select job
            );
            return result.ToList();
        }


        public static List<JobStatus> GetStatus(string ref_id)
        {
            using var serviceScope = JobHelp.CreateServiceScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();

            var result = (from job in dbContext.JobInfo
                          join status in dbContext.JobStatus on job.id equals status.jobId
                          where job.ref_id == ref_id
                          //orderby status.time
                          select status                         
            );

            return result.ToList();
        }


        public static List<FailedMessage> GetFailedMessage(string ref_id)
        {
            using var serviceScope = JobHelp.CreateServiceScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();

            var result = (from msg in dbContext.FailedMessage
                          where msg.ref_id == ref_id
                          select msg
                      );
            return result.ToList();
        }

        #endregion
    }
}
