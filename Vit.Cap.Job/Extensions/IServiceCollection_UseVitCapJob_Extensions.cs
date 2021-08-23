using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Vit.Cap.Job;
using Vit.Cap.Job.Entity;
using Vit.Core.Util.ConfigurationManager;
using Vit.Orm.EntityFramework;

namespace Vit.Extensions
{
    public static partial class IServiceCollection_UseVitCapJob_Extensions
    {
        class RabbitMqConfig
        {
            public string HostName;
            public int Port;
            public string UserName;
            public string Password;
        }

        /// <summary>
        /// 启用Cap。（若appsettings.json::Cap 配置不存在，则不启用）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="afterInitCap"></param>
        /// <returns></returns>
        public static IServiceCollection UseVitCapJob(this IServiceCollection services, Action<CapOptions> afterInitCap = null)
        {

            var ConnectionString = ConfigurationManager.Instance.GetStringByPath("Cap.ConnectionString");

            if (string.IsNullOrEmpty(ConnectionString)) return services;


            services.UseEntityFramework<VitCapJobDBContext>(new ConnectionInfo
            {
                type = "mysql",
                ConnectionString = ConnectionString
            });


            services.ConfigureApp(app =>
            {
                JobHelp.app = app;


                //确保创建数据库
                using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
                var context = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();
                if (context.Database.EnsureCreated())
                {
                    context.Database.ExecuteSqlRaw("alter table `" + JobLock.TableName + "` engine=InnoDB;");
                }
            });



            //配置Cap
            services.AddCap(x =>
            {
                // 配置Cap的本地消息记录库，用于服务端保存Published消息记录表；客户端保存Received消息记录表
                //x.UseMySql(ConnectionString);

                x.UseEntityFramework<VitCapJobDBContext>();


                var rabbitMqConfig = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<RabbitMqConfig>("Cap.RabbitMQ");
                x.UseRabbitMQ(mq =>
                {
                    mq.HostName = rabbitMqConfig.HostName;
                    mq.Port = rabbitMqConfig.Port;
                    mq.UserName = rabbitMqConfig.UserName;
                    mq.Password = rabbitMqConfig.Password;
                });




                //默认分组名，此值不配置时，默认值为当前程序集的名称
                if (!string.IsNullOrEmpty(JobHelp.DefaultCapGroup))
                    x.DefaultGroupName = JobHelp.DefaultCapGroup;


                //失败后的重试次数，默认50次；在FailedRetryInterval默认60秒的情况下，即默认重试50*60秒(50分钟)之后放弃失败重试
                x.FailedRetryCount = ConfigurationManager.Instance.GetByPath<int?>("Cap.FailedRetryCount") ?? 50;

                //失败后的重试间隔，默认60秒
                x.FailedRetryInterval = ConfigurationManager.Instance.GetByPath<int?>("Cap.FailedRetryInterval") ?? 60;

                //成功消息删除时间。默认24*3600秒
                x.SucceedMessageExpiredAfter = ConfigurationManager.Instance.GetByPath<int?>("Cap.SucceedMessageExpiredAfter") ??  (24 * 3600);


                //失败时的回调通知函数
                x.FailedThresholdCallback = failInfo =>
                {
                    //保存失败消息至数据库
                    using var serviceScope = JobHelp.CreateServiceScope();
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<VitCapJobDBContext>();

                    JobInfo jobInfo = null;
                    try
                    {
                        jobInfo = failInfo.Message.Value.ConvertBySerialize<JobInfo>();
                    }
                    catch
                    { 
                    }
                    

                    var failedMesage = new FailedMessage
                    {
                        MessageId = failInfo.Message.GetId(),
                        MessageType = failInfo.MessageType.ToString(),
                        Name = failInfo.Message.GetName(),
                        Headers = failInfo.Message.Headers.Serialize(),
                        Value = failInfo.Message.Value.Serialize(),
                        time = DateTime.Now,

                        ref_id = jobInfo?.ref_id,
                        ref_type = jobInfo?.ref_type,
                        ref_name = jobInfo?.ref_name,
                        ref_remark = jobInfo?.ref_remark

                    };

                    dbContext.Add(failedMesage);
                    dbContext.SaveChanges();

                    //Console.WriteLine($"失败回调:messageType: {failInfo.MessageType} ;messageName: {failInfo.Message.GetName()};    Headers:{ failInfo.Message.Headers.Serialize()}             ");
                };

                //The number of consumer thread connections. Default is 1
                x.ConsumerThreadCount = ConfigurationManager.Instance.GetByPath<int?>("Cap.ConsumerThreadCount") ?? 1;


                afterInitCap?.Invoke(x);

            });


            return services;
        }



    }
}
