using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Session;

namespace Investment1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Ensure the database is created and apply any pending migrations
            MigrateDatabase(host);

            host.Run(); // Start the web host
        }

        private static void MigrateDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Retrieve the database context
                    var context = services.GetRequiredService<InvestmentDbContext>();
                    context.Database.Migrate(); // Apply any pending migrations
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during migration
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        // Configure database context with SQL Server
                        services.AddDbContext<InvestmentDbContext>(options =>
                            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                        // Remove the DbInitializer registration if not needed
                        // services.AddTransient<IDbContextInitializer, DbInitializer>();

                        // Register email service with settings from configuration
                        services.Configure<SmtpSettings>(context.Configuration.GetSection("SmtpSettings"));
                        services.AddTransient<IEmailService, EmailService>();

                        // Register a background service for NAV updates
                        services.AddHostedService<NavUpdateService>();

                        // Add developer exception page for better error diagnostics
                        services.AddDatabaseDeveloperPageExceptionFilter();
                        services.AddControllersWithViews(); // Add MVC services

                        // Configure session services
                        services.AddDistributedMemoryCache(); // In-memory cache for session
                        services.AddSession(options =>
                        {
                            options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                            options.Cookie.HttpOnly = true; // Make session cookie HttpOnly
                            options.Cookie.IsEssential = true; // Make session cookie essential for the app
                        });

                        // Configure CORS (Cross-Origin Resource Sharing)
                        services.AddCors(options =>
                        {
                            options.AddPolicy("AllowAllOrigins",
                                builder =>
                                {
                                    builder.AllowAnyOrigin()
                                           .AllowAnyMethod()
                                           .AllowAnyHeader();
                                });
                        });
                    })
                    .Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        // Configure the HTTP request pipeline
                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage(); // Show detailed error pages in development
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error"); // Generic error handler for production
                            app.UseHsts(); // Enforce HTTPS
                        }

                        app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
                        app.UseStaticFiles(); // Serve static files (CSS, JS, images)
                        app.UseRouting(); // Add routing capabilities

                        // Enable CORS with the specified policy
                        app.UseCors("AllowAllOrigins");

                        // Use session middleware
                        app.UseSession();

                        app.UseAuthorization(); // Add authorization middleware

                        // Configure endpoint routing
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}"); // Default route

                            // Define additional routes
                            endpoints.MapControllerRoute(
                                name: "userAuthLogin",
                                pattern: "UserAuthentication/Login/{id?}",
                                defaults: new { controller = "UserAuthentication", action = "Login" });

                            endpoints.MapControllerRoute(
                                name: "userAuthForgotPassword",
                                pattern: "UserAuthentication/ForgotPassword/{id?}",
                                defaults: new { controller = "UserAuthentication", action = "ForgotPassword" });

                            endpoints.MapControllerRoute(
                                name: "userAuthResetPassword",
                                pattern: "UserAuthentication/ResetPassword/{id?}",
                                defaults: new { controller = "UserAuthentication", action = "ResetPassword" });

                            endpoints.MapControllerRoute(
                                name: "userRegistration",
                                pattern: "userregistration/{action=BasicUser}/{id?}",
                                defaults: new { controller = "UserRegistration" });

                            endpoints.MapControllerRoute(
                                 name: "support",
                                 pattern: "Support/{action=Create}/{investmentId?}",
                                  defaults: new { controller = "Support" });

                            endpoints.MapControllerRoute(
                                name: "portfolio",
                                pattern: "Portfolio/{action=Index}/{id?}",
                                defaults: new { controller = "Portfolio" });

                            endpoints.MapControllerRoute(
                                name: "updateSip",
                                pattern: "Portfolio/UpdateSIP/{id}",
                                defaults: new { controller = "Portfolio", action = "UpdateSIP" });

                            endpoints.MapControllers(); // Map attribute-routed controllers
                        });
                    });
                });
    }
}
