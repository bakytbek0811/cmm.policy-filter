using CMM.PolicyFilter.Configuration;
using CMM.PolicyFilter.Services;
using Ninject;

namespace CMM.PolicyFilter
{
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
                .UseServiceProviderFactory(new NinjectServiceProviderFactory())
                .ConfigureContainer<IKernel>((context, kernel) =>
                {
                    kernel.Load(new NinjectModule(context.Configuration));

                    kernel.Bind<IConfiguration>().ToConstant(context.Configuration);
                });
        }
    }
}