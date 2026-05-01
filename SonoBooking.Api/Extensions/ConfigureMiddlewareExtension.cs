using Asp.Versioning.ApiExplorer;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using SonoTracker.Application.Services.BackgroundJobs.Jobs;
using SonoTracker.Infrastructure.Context;
using SonoTracker.Domain;
using Serilog;
#pragma warning disable CS1591

namespace SonoTracker.Api.Extensions
{
    /// <summary>
    /// Pipeline Extensions
    /// </summary>
    public static class ConfigureMiddlewareExtension
    {

        public static IApplicationBuilder Configure(this IApplicationBuilder app, IConfiguration configuration, IApiVersionDescriptionProvider provider)
        {
            app.ConfigureCors();
            app.CreateDatabase();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.AddLocalization();
            app.UseFluentScheduler(configuration);
            app.SwaggerConfig(provider , configuration);
            app.UseHealthChecks("/probe");
            return app;
        }

        public static void ConfigureCors(this IApplicationBuilder app)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }


        public static void CreateDatabase(this IApplicationBuilder app)
        {
            try
            {
                using var scope =
                    app.ApplicationServices.CreateScope();
                using var context = scope.ServiceProvider.GetService<SonoTrackerDbContext>();

                var pendingMigrations = context!.Database.GetPendingMigrations();

                if (pendingMigrations.Any())
                {
                    Log.Information(MessagesConstants.ApplyingNewMigrations);
                    context.Database.Migrate();
                }
                else
                {
                    Log.Information(MessagesConstants.NoNewMigrations);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, MessagesConstants.ErrorMigratingDatabase);
                throw;
            }

        }


        public static void AddLocalization(this IApplicationBuilder app)
        {

            var supportedCultures = new[] { "en-US", "ar-OM" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures()
                .AddSupportedUICultures();
            app.UseRequestLocalization(localizationOptions);
        }

        private static void SwaggerConfig(this IApplicationBuilder app, IApiVersionDescriptionProvider provider , IConfiguration configuration)
        {
            var swaggerPath = configuration["SwaggerConfig:EndPoint"];
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/{swaggerPath}/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant()
                    );
                }
                options.DocExpansion(DocExpansion.None);
            });
        }

        public static void UseFluentScheduler(this IApplicationBuilder app, IConfiguration configuration)
        {
            var env = configuration["Environment"];
            if (env ==Common.StaticData.Environment.Development)
            {
                JobManager.Initialize(new MyRegistry());
            }

        }
    }
}
