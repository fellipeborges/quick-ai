using PropertiesStringifier;

namespace quick_ai.Model
{
    internal class Settings : StringifyProperties
    {
        public required string WindowTop { get; set; }
        public required string WindowLeft { get; set; }
        public required string WindowHeight { get; set; }
        public required string WindowWidth { get; set; }
        public required string WindowMaximized { get; set; }
        public required string AzureOpenAIEndpoint { get; set; }
        public required string AzureOpenAIKey { get; set; }
        public required string AzureOpenAIDeployment { get; set; }
    }
}
