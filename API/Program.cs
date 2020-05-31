using System;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        [Obsolete]
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = scope.ServiceProvider .GetRequiredService <ILoggerFactory> ();
                try
                {
                    var context = services.GetRequiredService<StoreContext>();
                    await context.Database.MigrateAsync();

                    //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductBrands ON");

                    await StoreContextSeed.SeedAsync (context, loggerFactory);

                    //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductBrands OFF");

                   //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductTypes OFF");
                }
                catch(Exception ex)
                {
                    var logger = loggerFactory .CreateLogger <Program>();
                    logger.LogError (ex, "An error occured during migration");
                }

                host.Run ();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
