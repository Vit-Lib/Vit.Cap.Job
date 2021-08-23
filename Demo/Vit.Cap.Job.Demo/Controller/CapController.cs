using Microsoft.AspNetCore.Mvc;
using System;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;
using System.Threading;
using Vit.Cap.Job.Attribute;
using Vit.Cap.Job.Entity;
using Vit.Extensions;
using DotNetCore.CAP;

namespace App.Controllers
{

    [Route("cap")]
    [ApiController]
    public class CapController : ControllerBase
    {

        [HttpGet("publish")]
        public ApiReturn<JobInfo> Publish(
            [FromQuery, SsExample("resPool01")] string jobGroup,
            [FromQuery, SsExample("解压文件")] string jobName,
            [FromQuery, SsExample("hello")] string jobArgs)
        { 
            JobInfo info = new JobInfo();
  
            info.jobGroup = jobGroup;
            info.jobName = jobName;
            info.jobArg = jobArgs;

            info.Publish();

            string msgBody = "[" + info.jobGroup + "]do job, jobId: " + info.id;
            Console.WriteLine("[publish] " + msgBody);

            return info;
        }


        #region Subscribe

        [NonAction]
        [CapJob("解压文件")]
        public void Subscribe(JobInfo info)
        { 
            info.InvokeJob(DoJob, "resPool01");
        }



        [NonAction]
        [CapJob("解压文件", "JobWaitForStart", "resPool02")]
        public void Subscribe2(JobInfo info)
        {
            info.InvokeJob(DoJob, "resPool02");
        }



        static void DoJob(JobInfo info, IServiceProvider ServiceProvider, object arg)
        {

            //Console.WriteLine("[InvokeJob] jobGuid " + info.jobGuid);
            //return;

            string workerName = (arg as string) ?? "resPool01";

            string msgBody = "[" + workerName + "]do job, jobId: " + info.id;
            Console.WriteLine("[begin] " + msgBody);


            float percent = 0;
            while (percent < 100)
            {
                Thread.Sleep(1000);

                percent += 10;

                var jobStatus = new JobStatus
                {
                    jobId = info.id,
                    percent = percent
                };

                jobStatus.Publish();
                Console.WriteLine("[status] " + percent + msgBody);
            }

            Console.WriteLine("[end] " + msgBody);
        }



   

        #endregion


    }


}
