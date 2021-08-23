using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Vit.Cap.Job;
using Vit.Cap.Job.Entity;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;


namespace Vit.Extensions
{
    public static partial class Job_Extensions
    {
        #region Publish Job

        

        /// <summary>
        /// 若失败，必须抛异常
        ///  (IServiceProvider serviceProvider, DotNetCore.CAP.ICapPublisher capBus, Action Commit) => {  Commit(); }
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <param name="customerAction"> </param>
        public static void Publish(this JobInfo jobInfo, Action<IServiceProvider, DotNetCore.CAP.ICapPublisher, Action> customerAction = null)
        {
            using var serviceScope = JobHelp.CreateServiceScope();
            DotNetCore.CAP.ICapPublisher _capBus = serviceScope.ServiceProvider.GetRequiredService<DotNetCore.CAP.ICapPublisher>();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();

            jobInfo.jobStatus = "等待处理";

            jobInfo.publicTime = DateTime.Now;

            jobInfo.id = 0;

            if (string.IsNullOrEmpty(jobInfo.jobGroup))
                jobInfo.jobGroup = JobHelp.DefaultJobGroup;



            using (var transaction = dbContext.Database.BeginTransaction(_capBus, autoCommit: false))
            {

                //(x.2)添加 jobInfo
                dbContext.JobInfo.Add(jobInfo);
                dbContext.SaveChanges();

                //(x.3)添加 jobLock
                var jobLock = new JobLock { jobId = jobInfo.id };
                dbContext.JobLock.Add(jobLock);
                dbContext.SaveChanges();


                Action OnCommit = () =>
                {
                    _capBus.Publish(JobHelp.GetCapEventName(jobInfo), jobInfo);
                    transaction.Commit();
                };

                if (customerAction == null)
                {
                    OnCommit();
                }
                else
                {
                    customerAction(serviceScope.ServiceProvider, _capBus, OnCommit);
                }
            }
        }
        #endregion


        #region Publish JobStatus       

        /// <summary>
        /// 若失败，必须抛异常
        ///  (IServiceProvider serviceProvider, DotNetCore.CAP.ICapPublisher capBus, Action Commit) => {  Commit(); }
        /// </summary>
        /// <param name="jobStatus"></param>
        /// <param name="customerAction"></param>
        /// <returns></returns>
        public static ApiReturn Publish(this JobStatus jobStatus, Action<IServiceProvider, DotNetCore.CAP.ICapPublisher, Action> customerAction = null)
        {
            using var serviceScope = JobHelp.CreateServiceScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();
            DotNetCore.CAP.ICapPublisher _capBus = serviceScope.ServiceProvider.GetRequiredService<DotNetCore.CAP.ICapPublisher>();

            using (var transaction = dbContext.Database.BeginTransaction(_capBus, autoCommit: false))
            {

                //(x.1)获取jobInfo
                var jobInfo = dbContext.JobInfo.FirstOrDefault(info => info.id == jobStatus.jobId);
                if (jobInfo == null)
                {
                    return false;
                }

                jobStatus.time = DateTime.Now;
                jobInfo.percent = jobStatus.percent;

                //(x.2)更新jobInfo
                if (jobInfo.jobStatus == "等待处理")
                {
                    jobInfo.jobStatus = "处理中";
                }

                if (jobInfo.beginTime == null)
                {
                    jobInfo.beginTime = jobStatus.time;
                }

                if (jobStatus.percent == 100.0)
                {
                    jobInfo.endTime = jobStatus.time;
                    jobInfo.jobStatus = "已处理";
                }

                dbContext.Update(jobInfo);
                dbContext.Add(jobStatus);
                dbContext.SaveChanges();



                Action OnCommit = () =>
                {
                    transaction.Commit();
                };

                if (customerAction == null)
                {
                    OnCommit();
                }
                else
                {
                    customerAction(serviceScope.ServiceProvider, _capBus, OnCommit);
                }

                return true;
            }
        }
        #endregion


        #region InvokeJob
     

        /// <summary>
        ///  
        /// </summary>
        /// <param name="info"></param>
        /// <param name="handle"> void DoJob(JobInfo info, IServiceProvider ServiceProvider, object arg) </param>
        /// <param name="arg"></param>
        public static void InvokeJob(this JobInfo info, Action<JobInfo, IServiceProvider, object> handle, object arg = null)
        {
            using var serviceScope = JobHelp.CreateServiceScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();

            var jobId = info.id;

            //套上事务
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                string sqlQuery = "select * from `" + JobLock.TableName + "` where jobId =" + jobId + " for update  nowait ";
                try
                {
                    var jobLock = dbContext.JobLock.FromSqlRaw(sqlQuery).FirstOrDefault();

                    if (jobLock == null)
                    {
                        throw new Exception("job locked by other worker.");
                    }

                    var jobInfo = dbContext.JobInfo.FirstOrDefault(i => i.id == jobId && i.jobStatus != "已处理");

                    if (jobInfo == null)
                    {
                        throw new Exception("job 已经处理完成，无需再次处理");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("jobId: " + info.id + " " + e.Message,e);
                    //Console.WriteLine("[error] jobGuid: " + info.jobGuid + " " + e.Message);
                    //throw;
                    throw new Exception("job locked by other worker.");
                }

                //这个时候查询已经被锁住了，可以做你要做操作
                handle(info, serviceScope.ServiceProvider, arg);

                //完成提交事务即可
                transaction.Commit();

            }
        }
        #endregion


    }
}
