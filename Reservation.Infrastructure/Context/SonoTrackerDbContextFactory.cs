using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SonoTracker.Infrastructure.Context
{
    /// <summary>
    /// Design-time factory for creating <see cref="SonoTrackerDbContext"/> when running EF Core tools (e.g. migrations).
    /// </summary>
    public class SonoTrackerDbContextFactory : IDesignTimeDbContextFactory<SonoTrackerDbContext>
    {
        public SonoTrackerDbContext CreateDbContext(string[] args)
        {
            // Prefer startup project directory (e.g. SonoTracker.Api) when running dotnet ef
            var basePath = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(basePath, "..", "SonoTracker.Api");
            if (Directory.Exists(apiPath))
                basePath = Path.GetFullPath(apiPath);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'Default' not found. Ensure appsettings.json is in the startup project (e.g. SonoTracker.Api).");

            var optionsBuilder = new DbContextOptionsBuilder<SonoTrackerDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SonoTrackerDbContext(optionsBuilder.Options);
        }
    }
}
