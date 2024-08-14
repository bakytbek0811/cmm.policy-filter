using System.Text.Json;
using CMM.PolicyFilter.Data;
using CMM.PolicyFilter.Entities;

namespace CMM.PolicyFilter.Services
{
    public interface IMessageService
    {
        void CheckMessageForPolicy(Message message);
        Message DeserializeMessage(string messageString);
    }

    public class MessageService : IMessageService
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async void CheckMessageForPolicy(Message message)
        {
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var _policyFilterService = scope.ServiceProvider.GetRequiredService<IPolicyFilterService>();

            var oldMessage = _context.Messages.FirstOrDefault(a => a.Id == message.Id);
            if (oldMessage == null)
            {
                throw new Exception($"Message with ID {message.Id} not found");
            }

            var filteredMessage = await _policyFilterService.Filter(message.Content);

            if (filteredMessage == oldMessage.Content)
            {
                return;
            }
            
            oldMessage.Content = filteredMessage;
            _context.Entry(oldMessage).Property(m => m.Content).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public Message DeserializeMessage(string messageString)
        {
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(messageString);
            if (jsonData == null)
            {
                throw new InvalidOperationException($"Cannot deserialize message: {messageString}");
            }

            var requiredFields = new[] { "id", "content", "originalContent", "fromUserId", "createdAt" };

            foreach (var field in requiredFields)
            {
                if (!jsonData.ContainsKey(field))
                {
                    throw new InvalidOperationException($"Missing required field: {field}");
                }
            }

            var message = JsonSerializer.Deserialize<Message>(messageString);

            if (message == null)
            {
                throw new InvalidOperationException($"Cannot deserialize message: {messageString}");
            }

            return message;
        }
    }
}