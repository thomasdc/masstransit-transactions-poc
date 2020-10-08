using System.Threading.Tasks;
using MassTransit;

namespace Api
{
    public class SomeEventConsumer : IConsumer<ISomeEvent>
    {
        public async Task Consume(ConsumeContext<ISomeEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
