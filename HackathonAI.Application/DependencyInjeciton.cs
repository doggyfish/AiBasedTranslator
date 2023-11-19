using Microsoft.Extensions.DependencyInjection;

namespace HackathonAI.Application
{
    public static class DependencyInjeciton
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services;
        }
    }
}
