using Microsoft.Extensions.Configuration;
using System;

namespace Notebook.Configuration
{
    public static class NotebookConfigurationManager
    {
        private static IConfiguration _configuration;
        public static string BasePath { get; set; }

        public static IConfiguration GetConfiguration(string developer = "")
        {
            if (_configuration == null)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(BasePath ?? AppContext.BaseDirectory)  //TODO check string.IsNullOrEmpty
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                if (string.IsNullOrEmpty(developer))
                {
                    configuration.AddJsonFile($"appsettings.development.{developer.ToLower()}.json", optional: true);
                }

                _configuration = configuration.Build();
            }

            return _configuration;
        }
    }
}
