using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// Program.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

using BreweryAPI.Services.Interfaces;
using BreweryAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using BreweryAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BreweryAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    // Use extension methods on the 'logging' parameter
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureServices((context, services) =>
                {
                    // Add services
                    services.AddMemoryCache();
                    services.AddHttpClient<IBreweryRepository, BreweryRepository>(client =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                    });

                    // DI
                    services.AddSingleton<ICacheProvider, CacheProvider>();
                    services.AddScoped<IBreweryService, BreweryService>();

                    // Controllers + versioning
                   // services.AddControllers();
                    services.AddApiVersioning(options =>
                    {
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ReportApiVersions = true;
                        options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    });
                   

                     services.AddDbContext<BreweryDbContext>(opt =>
                         opt.UseSqlite(context.Configuration.GetConnectionString("Default")));
                     services.AddHttpClient<IBreweryRepository, BreweryRepositorySqlite>();
                    
                })

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}





