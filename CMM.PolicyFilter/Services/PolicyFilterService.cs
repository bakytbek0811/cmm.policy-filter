using CMM.PolicyFilter.Entities;

namespace CMM.PolicyFilter.Services
{
    public interface IPolicyFilterService
    {
        Task<string> Filter(string text);
    }

    public class PolicyFilterService : IPolicyFilterService
    {
        private readonly IGptService _gptService;

        public PolicyFilterService(IGptService gptService)
        {
            _gptService = gptService;
        }

        public async Task<string> Filter(string text)
        {
            var messages = new[]
            {
                new GptMessage
                {
                    Content =
                        $"Сообщение от клиента: {text}\n\nВ случае, если в сообщении содержатся упоминания о политике, ты должен каждый символ (кроме пробела) в фразе о политике заменить на '*'. \nНапример, фраза “Голосуем за Трампа” должна быть преобразована в “******** ** ******“. \nВерни только обновленное сообщение, без никаких информации!",
                    Role = "user"
                }
            };

            var gptResponse = await this._gptService.GenerateText(messages);
            var filteredText = gptResponse.Choices[0].Message.Content;

            return filteredText;
        }
    }
}