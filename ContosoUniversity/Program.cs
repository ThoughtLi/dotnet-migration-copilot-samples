
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Data;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

    namespace ContosoUniversity
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Configure request size limits (from Web.config httpRuntime and requestFiltering)
                builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
                {
                    options.MultipartBodyLengthLimit = 10485760; // 10MB (maxAllowedContentLength from Web.config)
                });

                builder.Services.Configure<IISServerOptions>(options =>
                {
                    options.MaxRequestBodySize = 10485760; // 10MB
                });

                //Added Services

                var app = builder.Build();

                // Initialize database with Entity Framework 6
                InitializeDatabase(app.Configuration);

                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                //Added Middleware

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }

            private static void InitializeDatabase(IConfiguration configuration)
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
                    optionsBuilder.UseSqlServer(connectionString);
using (var context = new SchoolContext(optionsBuilder.Options))
                    {
                        DbInitializer.Initialize(context);
                    }
                }
            }
        }

        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
