using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class NewBlogRegisteredConsumer : IConsumer<INewBlogRegistered>
    {
        private readonly ILogger<NewBlogRegisteredConsumer> _logger;

        public NewBlogRegisteredConsumer(ILogger<NewBlogRegisteredConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<INewBlogRegistered> context)
        {
            _logger.LogInformation($"A new blog with ID {context.Message.BlogId} was registered!");
            await Task.CompletedTask;
        }
    }
}
