using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Notebook.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNotebookConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var spConfigSection = configuration.GetSection("NotebookConnection");
            services.Configure<NotebookSettings>(spConfigSection);
        }
    }
}
