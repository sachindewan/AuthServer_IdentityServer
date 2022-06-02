using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobsApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobsApi
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://localhost:50012";
                options.Audience = "jobsapi";
            });
            services.AddSingleton<IConfig>(Configuration.GetSection("CustomConfig")?.Get<Config>());
            AddDbContexts(services);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobsApi", Version = "v1" });
            });
        }

        private void AddDbContexts(IServiceCollection services)
        {
            var debugLogging = new Action<DbContextOptionsBuilder>(opt =>
            {
#if DEBUG
                //this will log ef generated sql commands to the console
                opt.UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                }));
                //this will log the params for those commands to the console
                opt.EnableSensitiveDataLogging();
                opt.EnableDetailedErrors();
            });
            services.AddDbContext<JobsContext>(opt =>
            {
                opt.UseSqlite(Configuration.GetConnectionString("sqldb-job") ?? "name=Jobs");
                debugLogging(opt);
            },ServiceLifetime.Transient);
#endif
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobsApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            TryRunMigrationAndSeeded(app);
        }

        private void TryRunMigrationAndSeeded(IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<IConfig>();
            if (config.RunDbMigration)
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dbcontext = scope.ServiceProvider.GetRequiredService<JobsContext>();
                    dbcontext.Database.Migrate();
                }
            }
            if (config.SeedDataBase)
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dbcontext = scope.ServiceProvider.GetRequiredService<JobsContext>();
                    DatabaseInializer.Initialize(dbcontext);
                }
            }
        }
    }
}
