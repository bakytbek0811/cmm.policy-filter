using CMM.PolicyFilter.Data;
using CMM.PolicyFilter.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CMM.PolicyFilter.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMq:HostName"],
                UserName = configuration["RabbitMq:UserName"],
                Password = configuration["RabbitMq:Password"]
            };

            services.AddSingleton(factory);
            services.AddSingleton<IConnection>(sp => sp.GetRequiredService<ConnectionFactory>().CreateConnection());
            services.AddSingleton<IModel>(sp => sp.GetRequiredService<IConnection>().CreateModel());

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ChatMainDb")));

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IGptService, GptService>();
            services.AddScoped<IPolicyFilterService, PolicyFilterService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddSingleton<IMessageQueueService, MessageQueueService>();

            return services;
        }
    }
}