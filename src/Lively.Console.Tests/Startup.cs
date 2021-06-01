using Microsoft.Extensions.DependencyInjection;

namespace Lively.Console.Tests
{
    // Required for interface resolver tests
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) { }
    }
}
