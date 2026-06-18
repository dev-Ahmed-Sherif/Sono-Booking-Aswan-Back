using Asp.Versioning.ApiExplorer;
using Hangfire;
using Serilog;
using SonoBooking.Api.Extensions;
using SonoBooking.Api.MiddleWares;
using SonoBooking.Api.Seed;
using SonoBooking.Application.DependencyExtension;
using SonoBooking.Application.Services.BackgroundJobs.Housing.Reservations;

namespace SonoBooking.Api
{
    /// <summary>
    /// Start Point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configuration Properties
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        /// <summary>
        /// Kick Off
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            //Log.Logger = BaseLoggerConfiguration
            //    .CreateLoggerConfiguration(Configuration["ApplicationName"])
            //    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            //    .WriteToSql(Configuration["LoggingDbConnectionString"])
            //    .WriteToSeq(Configuration["LoggingSeqUrl"])
            //    .CreateLogger();

            try
            {
                Log.Information("-----Starting web host at  Api------");

                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                builder.Services.RegisterServices(builder.Configuration);

                var app = builder.Build();

                var shell = new Shell();
                app.UseCors("policy");
                shell.ConfigureHttp(app, app.Environment);
                Shell.Start(shell);
                app.UseStaticFiles();
                app.Configure(builder.Configuration, app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                app.UseHangfireDashboard("/sono-booking-Jobs");
                //app.UseHangfireServer();
                ReservationNoShowJob.RegisterDailySchedule();
                ReservationCheckoutJob.RegisterDailySchedule();
                app.ConfigureCustomMiddleware();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                await DatabaseSeed.SeedIdentityAsync(app);
                await DatabaseSeed.SeedLookupsAsync(app);

                await app.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
