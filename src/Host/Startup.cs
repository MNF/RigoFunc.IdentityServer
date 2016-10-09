using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Host.Configuration;
using Host.Models.Domains;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using RigoFunc.ApiCore;
using RigoFunc.ApiCore.Default;
using RigoFunc.ApiCore.Filters;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework;

namespace Host {
    public class Startup {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env) {
            _environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration["Data:Default:ConnectionString"];

            services.AddDbContext<AppDbContext>(builder =>
                builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)));

            // Add Asp.Net Core Identity
            services.AddIdentity<AppUser, IdentityRole<int>>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext, int>()
            .AddDefaultTokenProviders()
            .AddUserClaimsPrincipalFactory();

            // Add Identity Server
            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "idsrv3test.pfx"), "idsrv3test");
            services.AddIdentityServer()
                .SetSigningCredential(cert)
                .AddConfigurationStore(builder =>
                    builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddConfigurationStoreCache()
                .AddOperationalStore(builder =>
                    builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetCoreIdentity<AppUser>();

            services.AddScoped<IApiResultHandler, DefaultApiResultHandler>();
            services.AddScoped<IApiExceptionHandler, DefaultApiExceptionHandler>();

            // for the UI
            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                }).AddMvcOptions(options => {
                    options.Filters.Add(typeof(ApiResultFilterAttribute));
                    options.Filters.Add(typeof(ApiExceptionFilterAttribute));
                }).AddApiHelp();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Setup Databases
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                serviceScope.ServiceProvider.GetService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
                EnsureSeedData(serviceScope.ServiceProvider.GetService<ConfigurationDbContext>());

                var options = serviceScope.ServiceProvider.GetService<DbContextOptions<PersistedGrantDbContext>>();
                var tokenCleanup = new TokenCleanup(new TokenCleanupOptions {
                    DbContextOptions = options,
                    Interval = 30000,
                });
                tokenCleanup.Start();
            }

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseApiHelpUI();

            app.UseMvcWithDefaultRoute();
        }

        private static void EnsureSeedData(ConfigurationDbContext context) {
            if (!context.Clients.Any()) {
                foreach (var client in Clients.Get()) {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.Scopes.Any()) {
                foreach (var client in Scopes.Get()) {
                    context.Scopes.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}