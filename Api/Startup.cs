using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<BloggingContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("BloggingDatabase")));

            services.AddMassTransit(x =>
            {
                x.AddTransactionalBus();

                x.AddConsumersFromNamespaceContaining<Startup>();
                x.AddBus(provider => Bus.Factory.CreateUsingAmazonSqs(cfg =>
                {
                    cfg.Host("us-east-1", config =>
                    {
                        config.Config(new AmazonSQSConfig {ServiceURL = "http://localhost:4666"});
                        config.Config(new AmazonSimpleNotificationServiceConfig {ServiceURL = "http://localhost:4666"});
                        config.AccessKey("does_not");
                        config.SecretKey("matter");
                    });

                    const string topicName = "BlogsTopic";
                    cfg.Message<INewBlogRegistered>(m => m.SetEntityName(topicName));

                    cfg.ReceiveEndpoint("BlogsTopic_subscription_queue", e =>
                    {
                        e.Subscribe(topicName);
                        e.ConfigureConsumer<NewBlogRegisteredConsumer>(provider);
                    });
                }));
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
