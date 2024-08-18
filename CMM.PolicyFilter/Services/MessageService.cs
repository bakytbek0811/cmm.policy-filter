using System.Text.Json;
using CMM.PolicyFilter.Data;
using CMM.PolicyFilter.Entities;

namespace CMM.PolicyFilter.Services
{
    public interface IMessageService
    {
        Task CheckMessageForPolicy(Message message);
        Message DeserializeMessage(string messageString);
    }

    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        private readonly IPolicyFilterService _policyFilterService;

        public MessageService(AppDbContext context,
            IPolicyFilterService policyFilterService)
        {
            // _serviceProvider = serviceProvider;
            _context = context;
            _policyFilterService = policyFilterService;
        }

        public async Task CheckMessageForPolicy(Message message)
        {
            // using var scope = _serviceProvider.CreateScope();
            // var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // var _policyFilterService = scope.ServiceProvider.GetRequiredService<IPolicyFilterService>();

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