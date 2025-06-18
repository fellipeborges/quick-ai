using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using quick_ai.Constants;
using quick_ai.Model;

namespace quick_ai.Service
{
    internal class AiService
    {
        private static readonly bool IsMocked = false;

        public static async Task<AiResponse> ProcessAsync(List<ChatInteration> chatInterations) =>
            await MakeAiCallAsync(BuildChatMessages(chatInterations), MockConstants.RESPONSE_GENERAL);

        internal static async Task<AiResponse> MakeAiCallAsync(List<ChatMessage> messages, string responseWhenMocked)
        {
            return IsMocked ?
                await MakeMockedAiCallAsync(responseWhenMocked) :
                await MakeAzureAiCallAsync(messages);
        }

        internal static List<ChatMessage> BuildChatMessages(List<ChatInteration> chatInterations, string initialInstructions = "")
        {
            List<ChatMessage> chatMessages = [];

            if (!string.IsNullOrWhiteSpace(initialInstructions))
                chatMessages.Add(new SystemChatMessage(initialInstructions));

            chatInterations
                .Where(ci => !ci.Temporary)
                .OrderBy(ci => ci.Position)
                .ToList()
                .ForEach(ci =>
                {
                    if (ci.Sender == ChatConstants.CHAT_SENDER_USER)
                        chatMessages.Add(new UserChatMessage(ci.Message));
                    else if (ci.Sender == ChatConstants.CHAT_SENDER_AI)
                        chatMessages.Add(new SystemChatMessage(ci.Message));
                });

            return chatMessages;
        }

        private static async Task<AiResponse> MakeAzureAiCallAsync(List<ChatMessage> messages)
        {
            string apiResponseContent = string.Empty;

            try
            {
                ChatClient chatClient = CreateChatClient();
                var options = new ChatCompletionOptions
                {
                    Temperature = (float)0.7,
                    MaxOutputTokenCount = 800,
                    TopP = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };

                ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);
                if (completion == null || completion.Content.Count == 0)
                    return AiResponse.AsFailed("Failed to complete chat request.");

                apiResponseContent = completion.Content[0].Text;
                return AiResponse.AsSuccess(apiResponseContent);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                errorMessage += string.IsNullOrWhiteSpace(apiResponseContent) ? "" : $" API response content: {apiResponseContent}";
                return AiResponse.AsFailed(errorMessage);
                throw;
            }
        }

        private static async Task<AiResponse> MakeMockedAiCallAsync(string mockedResponse)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(2500);
                return new AiResponse
                {
                    Success = true,
                    Content = mockedResponse,
                    ErrorMessage = ""
                };
            });
        }

        private static ChatClient CreateChatClient()
        {
            Settings settings = SettingsService.GetSettings();
            if (string.IsNullOrEmpty(settings.AzureOpenAIEndpoint) ||
                string.IsNullOrEmpty(settings.AzureOpenAIKey) ||
                string.IsNullOrEmpty(settings.AzureOpenAIDeployment))
            {
                throw new Exception("Azure OpenAI settings are not configured. Please configure them in the settings window.");
            }

            AzureKeyCredential credential = new(settings.AzureOpenAIKey);
            AzureOpenAIClient azureClient = new(new Uri(settings.AzureOpenAIEndpoint), credential);
            return azureClient.GetChatClient(settings.AzureOpenAIDeployment);
        }
    }
}
