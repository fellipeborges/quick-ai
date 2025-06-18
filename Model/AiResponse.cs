namespace quick_ai.Model
{
    internal class AiResponse
    {
        public required string Content { get; set; }
        public string ContentSecondary { get; set; } = string.Empty;
        public required bool Success { get; set; }
        public required string ErrorMessage { get; set; }
        internal static AiResponse AsFailed(string errorMessage) =>
            new()
            {
                Content = string.Empty,
                Success = false,
                ErrorMessage = errorMessage
            };
        internal static AiResponse AsSuccess(string content) =>
            new()
            {
                Content = content,
                Success = true,
                ErrorMessage = string.Empty
            };

        internal static AiResponse AsSuccess(string content, string secondaryContent) =>
            new()
            {
                Content = content,
                ContentSecondary = secondaryContent,
                Success = true,
                ErrorMessage = string.Empty
            };
    }
}
