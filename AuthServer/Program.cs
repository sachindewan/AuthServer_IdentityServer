using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using IdentityServer4.EntityFramework.Storage;

namespace AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            { 
                var services = scope.ServiceProvider;
                try
                {
                    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
                    var persistedGrantDbContext = services.GetRequiredService<PersistedGrantDbContext>();
                    var configurationDbContext = services.GetRequiredService<ConfigurationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    if (applicationDbContext.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
                    {
                        applicationDbContext.Database.Migrate();
                    }
                    if (persistedGrantDbContext.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
                    {
                        persistedGrantDbContext.Database.Migrate();
                    }
                    if (configurationDbContext.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
                    {
                        configurationDbContext.Database.Migrate();
                    }
                    Log.Information("Seeding database...");
                    SeedData.EnsureSeedData(configurationDbContext, userManager, roleManager);
                    Log.Information("Done seeding database.");
                    Log.Information("Starting host...");
                    host.Run();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation(ex, "An error occured during migration");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
