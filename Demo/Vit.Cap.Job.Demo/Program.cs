using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Vit.Extensions;   //----添加代码1

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args) 
            .UseUrlsFromConfig()
            .UseStartup<Startup>();
    }
}
