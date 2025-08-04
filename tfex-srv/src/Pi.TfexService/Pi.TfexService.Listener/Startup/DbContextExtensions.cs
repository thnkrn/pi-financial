using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.TfexService.Infrastructure;
using Pi.TfexService.Migrations;
using Serilog;

namespace Pi.TfexService.Listener.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            return services;
        }
    }
}

