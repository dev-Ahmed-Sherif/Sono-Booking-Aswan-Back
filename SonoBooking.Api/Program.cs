using Serilog;
using Serilog.Events;
using SonoBooking.Api.Seed;
using SonoTracker.Common.Extensions;

namespace SonoTracker.Api
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
                var host = CreateHostBuilder(args).Build();

                await DatabaseSeed.SeedIdentityAsync(host);

                await host.RunAsync();
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
        /// <summary>
        /// Web Host Builder
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>();
                }).UseSerilog();
    }
}
