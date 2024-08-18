using DotNetEnv;
using DotNetEnv.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;

namespace RelationAnalysis.Migrations
{
    public class ConsoleStartup
    {
        public ConsoleStartup()
        {
            Env.Load();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddDotNetEnv()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            

            //.. for test
            Console.WriteLine(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")).UseLazyLoadingProxies();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
       
        }
    }
}