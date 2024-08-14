// docker run -d --name chat_postgres -e POSTGRES_PASSWORD=78SoKg55hefD6y4e0raN -p 5400:5432 postgres

using CMM.PolicyFilter.Extensions;
using CMM.PolicyFilter.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var messageQueueService = host.Services.GetRequiredService<IMessageQueueService>();
        messageQueueService.StartListening();

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.AddDatabase(configuration);
                services.AddApplicationServices();
                services.AddRabbitMq(configuration);
            });
}