using System;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SonoTracker.Application.DependencyExtension;

namespace SonoTracker.Application.Services.BackgroundJobs.Jobs
{
    public class TestJob : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestJob> _logger;
        public TestJob()
        {
            _configuration = Shell.RootInjector.GetService<IConfiguration>();
            _logger = Shell.RootInjector.GetRequiredService<ILogger<TestJob>>();
        }

        #region Public Methods

        public Task ExecuteTestJob()
        {
            return Task.CompletedTask;
        }

        public void Execute()
        {
            try
            {
                ExecuteTestJob();
            }
            catch (Exception e)
            {
                var serializerSettings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                var exception = new
                {
                    e.Message,
                    e.StackTrace,
                    e.InnerException
                };
                var exceptionJson = JsonConvert.SerializeObject(exception, serializerSettings);
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Private Methods



        #endregion



    }
}
