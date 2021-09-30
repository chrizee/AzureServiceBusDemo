using System.Threading.Tasks;

namespace AzureServiceBus.Sender.Services
{
    public interface IQueueService
    {
        Task SendMessageAsync<T>(T serviceBusmessage, string queue);
    }
}