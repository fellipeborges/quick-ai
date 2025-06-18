using PropertiesStringifier;

namespace quick_ai.Model
{
    internal class ChatInteration : StringifyProperties
    {
        public required int Position { get; set; }
        public required string Message { get; set; }
        public required string Sender { get; set; }
        public required bool Temporary { get; internal set; }
    }
}
