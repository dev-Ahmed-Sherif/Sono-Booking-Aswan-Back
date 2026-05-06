using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SonoBooking.Infrastructure.Context
{
    /// <summary>
    /// Design-time factory for creating <see cref="SonoBookingDbContext"/> when running EF Core tools (e.g. migrations).
    /// </summary>
    public class SonoBookingDbContextFactory : IDesignTimeDbContextFactory<SonoBookingDbContext>
    {
        public SonoBookingDbContext CreateDbContext(string[] args)
        {
            // Prefer startup project directory (e.g. SonoTracker.Api) when running dotnet ef
            var basePath = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(basePath, "..", "SonoBooking.Api");
            if (Directory.Exists(apiPath))
                basePath = Path.GetFullPath(apiPath);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'Default' not found. Ensure appsettings.json is in the startup project (e.g. SonoBooking.Api).");

            var optionsBuilder = new DbContextOptionsBuilder<SonoBookingDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SonoBookingDbContext(optionsBuilder.Options);
        }
    }
}
