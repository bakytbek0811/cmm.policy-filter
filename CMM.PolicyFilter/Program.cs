// docker run -d --name chat_postgres -e POSTGRES_PASSWORD=78SoKg55hefD6y4e0raN -p 5400:5432 postgres

using CMM.PolicyFilter.Extensions;
using CMM.PolicyFilter.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var queueService = host.Services.GetRequiredService<IMessageQueueService>();
        queueService.StartListening();

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    // logging.ClearProviders();
                    // logging.AddConsole();
                    // logging.SetMinimumLevel(LogLevel.Debug);
                }).ConfigureServices((context, services) =>
                {
                    services.AddDatabase(context.Configuration);
                    services.AddRabbitMq(context.Configuration);
                    services.AddApplicationServices();
                })
            // .UseServiceProviderFactory(new NinjectServiceProviderFactory())
            // .ConfigureContainer<IKernel>((context, kernel) => { kernel.Load(new DataModule(context.Configuration)); })
            ;
    }
}