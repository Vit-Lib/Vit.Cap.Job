using DotNetCore.CAP.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(options =>
            {
                //使用自定义异常处理器
                //options.Filters.Add<Sers.Serslot.ExceptionFilter.ExceptionFilter>();

            }).AddJsonOptions(options =>
            {
                //options.JsonSerializerOptions.AddConverter_Newtonsoft();
                //options.JsonSerializerOptions.AddConverter_DateTime();

                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
                options.JsonSerializerOptions.IncludeFields = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

             
 


            //配置Cap
            services.AddCap(x =>
            {         

                // 配置Cap的本地消息记录库，用于服务端保存Published消息记录表；客户端保存Received消息记录表
                x.UseMySql(Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Cap.ConnectionString"));


                x.UseRabbitMQ(mq => {
                    mq.HostName = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Cap.RabbitMQ.HostName");//RabbitMQ服务器地址
                    mq.Port = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("Cap.RabbitMQ.Port");
                    mq.UserName = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Cap.RabbitMQ.UserName");
                    mq.Password = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Cap.RabbitMQ.Password");
                });


                //使用Dashboard，这是一个Cap的可视化管理界面；默认地址:http://localhost:端口/cap
                x.UseDashboard(dashoptions =>
                {
                    //dashoptions.AppPath = "applicationpath";
                    //dashoptions.PathMatch = "/cap";
                    dashoptions.Authorization = new[] { new CapDashboardFilter() };

                });

                //默认分组名，此值不配置时，默认值为当前程序集的名称
                //x.DefaultGroup = "m";

                //失败后的重试次数，默认50次；在FailedRetryInterval默认60秒的情况下，即默认重试50*60秒(50分钟)之后放弃失败重试
                //x.FailedRetryCount = 10;

                //失败后的重试间隔，默认60秒
                //x.FailedRetryInterval = 30;

                //设置成功信息的删除时间默认24*3600秒
                //x.SucceedMessageExpiredAfter = 60 * 60;

                //The number of consumer thread connections. Default is 1
                x.ConsumerThreadCount = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int?>("Cap.ConsumerThreadCount") ??1;

            });

        }



        public class CapDashboardFilter : DotNetCore.CAP.Dashboard.IDashboardAuthorizationFilter
        {
            public async Task<bool> AuthorizeAsync(DashboardContext context)
            {
                return true;
            }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }
}
