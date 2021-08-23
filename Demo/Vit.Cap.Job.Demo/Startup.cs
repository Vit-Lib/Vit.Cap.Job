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



            //启用VitCapJob
            //services.UseVitCapJob();

            //启用VitCapJob 并启用DashBoard
            services.UseVitCapJob((x)=>{

                //使用Dashboard，这是一个Cap的可视化管理界面；默认地址:http://localhost:端口/cap
                x.UseDashboard(dashoptions =>
                {
                    //dashoptions.AppPath = "applicationpath";
                    //dashoptions.PathMatch = "/cap";
                    dashoptions.Authorization = new[] { new CapDashboardFilter() };

                });
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
