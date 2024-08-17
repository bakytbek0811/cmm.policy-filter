using CMM.PolicyFilter.Data;
using CMM.PolicyFilter.Services;
using Microsoft.EntityFrameworkCore;
using Ninject;
using Ninject.Modules;
using RabbitMQ.Client;

namespace CMM.PolicyFilter.Configuration;

public class DataModule : NinjectModule
{
    private readonly IConfiguration _configuration;

    public DataModule(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public override void Load()
    {
        this.Bind<DbContextOptions<AppDbContext>>().ToMethod(context =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("ChatMainDb"));
            return optionsBuilder.Options;
        }).InSingletonScope();
        
        Bind<AppDbContext>().ToSelf().InTransientScope();
        
        this.Bind<ConnectionFactory>().ToMethod(_ => new ConnectionFactory()
        {
            HostName = _configuration["RabbitMq:HostName"],
            UserName = _configuration["RabbitMq:UserName"],
            Password = _configuration["RabbitMq:Password"]
        }).InSingletonScope();
        this.Bind<IConnection>().ToMethod(_ => this.Kernel.Get<ConnectionFactory>().CreateConnection())
            .InSingletonScope();
        this.Bind<IModel>().ToMethod(_ => this.Kernel.Get<IConnection>().CreateModel()).InSingletonScope();
        
        this.Bind<IGptService>().To<GptService>().InSingletonScope();
        this.Bind<IPolicyFilterService>().To<PolicyFilterService>().InSingletonScope();
        this.Bind<IMessageService>().To<MessageService>().InSingletonScope();
        this.Bind<IMessageQueueService>().To<MessageQueueService>().InSingletonScope();
    }
}