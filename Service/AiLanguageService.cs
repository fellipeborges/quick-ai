using OpenAI.Chat;
using quick_ai.Constants;
using quick_ai.Model;
using System.Text.Json;

namespace quick_ai.Service
{
    internal class AiLanguageService : AiService
    {
        public static new async Task<AiResponse> ProcessAsync(List<ChatInteration> chatInterations)
        {
            List<ChatMessage> chatMessages = BuildChatMessages(chatInterations);
            AiResponse aiResponse = await MakeAiCallAsync(chatMessages, MockConstants.RESPONSE_GRAMMAR);

            if (!aiResponse.Success)
                return AiResponse.AsFailed(aiResponse.ErrorMessage);

            if (!ValidateJson(aiResponse.Content, out string validationError))
                return AiResponse.AsFailed($"Invalid JSON response format: {validationError}");

            using JsonDocument jsonDoc = JsonDocument.Parse(aiResponse.Content);
            string content = jsonDoc.RootElement.GetProperty("revisedText").GetString() ?? "";

            string secondaryContent = "";
            jsonDoc.RootElement.TryGetProperty("summary", out JsonElement summaryElement);
            foreach (JsonElement element in summaryElement.EnumerateArray())
            {
                secondaryContent += $"{element.GetProperty("before").GetString()} → {element.GetProperty("after").GetString()} \n\n";
            }

            return AiResponse.AsSuccess(content, secondaryContent);
        }

        private static bool ValidateJson(string jsonText, out string error)
        {
            error = string.Empty;

            try
            {
                using JsonDocument jsonDoc = JsonDocument.Parse(jsonText);
                if (!jsonDoc.RootElement.TryGetProperty("revisedText", out JsonElement revisedTextElement))
                    throw new Exception("Missing 'revisedText' property");

                if (revisedTextElement.ValueKind != JsonValueKind.String)
                    throw new Exception("Property 'revisedText' must be a string");

                if (!jsonDoc.RootElement.TryGetProperty("summary", out JsonElement summaryElement))
                    throw new Exception("Missing 'summary' property");

                if (summaryElement.ValueKind != JsonValueKind.Array)
                    throw new Exception("Property 'summary' must be an array");

                foreach (JsonElement element in summaryElement.EnumerateArray())
                {
                    if (!element.TryGetProperty("before", out JsonElement beforeElement) || beforeElement.ValueKind != JsonValueKind.String)
                        throw new Exception("Each summary item must contain a 'before' string property");

                    if (!element.TryGetProperty("after", out JsonElement afterElement) || afterElement.ValueKind != JsonValueKind.String)
                        throw new Exception("Each summary item must contain a 'after' string property");
                }

                return true;
            }
            catch (Exception ex)
            {
                error = $"Invalid JSON format: {ex.Message}";
                return false;
            }
        }

        private static List<ChatMessage> BuildChatMessages(List<ChatInteration> chatInterations)
        {
            const string INITIAL_INSTRUCTIONS = @"
                    You are an AI assistant that helps people write better in the text language. It will be provided a text in a foreign language and you are must identify which language is it, analyze the text, correct it, make it gramatically accurate and make it more natural sounding. The text is meant to be used in a professional context but it does not need to be too much formal.
                    Your answer must be in json format and must contain:
                        - revisedText: the the text in the target language
                        - summary: an array containing all that was corrected in the original text and how it was changed in the final text
                    Your answer must not contain markdown elements, code blocks or any other formatting. The json must be valid and must not contain any additional text or explanation.
                    
                    The json must be formatted as follows:
                    {
                        ""revisedText"": ""Final text in target language"",
                        ""summary"": [
                            {""before"": ""Original text"", ""after"": ""Final text""},
                            {""before"": ""Original text"", ""after"": ""Final text""}
                        ]
                    }
                ";

            return BuildChatMessages(chatInterations, INITIAL_INSTRUCTIONS);
        }
    }
}
