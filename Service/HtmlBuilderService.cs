using Markdig;
using quick_ai.Constants;
using quick_ai.Model;
using System.Text;

namespace quick_ai.Service
{
    internal static class HtmlBuilderService
    {
        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        public static string BuildHtml(List<ChatInteration> chatInterations)
        {
            string htmlTemplate = GetHtmlTemplate();
            var chatMessagesStrBuilder = new StringBuilder();

            chatInterations
                .OrderBy(ci => ci.Position)
                .ToList()
                .ForEach(ci =>
                {
                    string html = ci.Sender == ChatConstants.CHAT_SENDER_AI ? GetMessageReceivedTemplate() : GetMessageSentTemplate();
                    html = html.Replace("$ID$", ci.Position == chatInterations.Count ? "last-message" : string.Empty);
                    html = html.Replace("$MESSAGE$", Markdown.ToHtml(ci.Message.Replace("\r\n", "\n\n"), MarkdownPipeline));
                    chatMessagesStrBuilder.Append(html);
                });

            return htmlTemplate.Replace("$CHAT-MESSAGES$", chatMessagesStrBuilder.ToString());
        }

        private static string GetMessageReceivedTemplate()
        {
            return @"
                <div class=""message received"">
                    <div class=""message-content"" id=""$ID$"">
                        $MESSAGE$
                        <button class=""copy-button"" onclick=""copyToClipboard(this.closest('.message-content'), this)"">
                            <svg class=""copy-icon"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                                <path d=""M8 4V16C8 16.5304 8.21071 17.0391 8.58579 17.4142C8.96086 17.7893 9.46957 18 10 18H18C18.5304 18 19.0391 17.7893 19.4142 17.4142C19.7893 17.0391 20 16.5304 20 16V7.242C20 6.97556 19.9467 6.71181 19.8433 6.46624C19.7399 6.22068 19.5885 5.99824 19.398 5.812L16.188 2.602C16.0018 2.41147 15.7793 2.26013 15.5338 2.15673C15.2882 2.05333 15.0244 2.00001 14.758 2H10C9.46957 2 8.96086 2.21071 8.58579 2.58579C8.21071 2.96086 8 3.46957 8 4Z"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
                                <path d=""M16 18V20C16 20.5304 15.7893 21.0391 15.4142 21.4142C15.0391 21.7893 14.5304 22 14 22H6C5.46957 22 4.96086 21.7893 4.58579 21.4142C4.21071 21.0391 4 20.5304 4 20V8C4 7.46957 4.21071 6.96086 4.58579 6.58579C4.96086 6.21071 5.46957 6 6 6H8"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
                            </svg>
                        </button>
                    </div>
                </div>
            ";
        }

        private static string GetMessageSentTemplate()
        {
            return @"
                <div class=""message sent"">
                    <div class=""message-content"">
                        $MESSAGE$
                    </div>
                </div>";
        }

        private static string GetHtmlTemplate()
        {
            return @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <style>
                        body {
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            font-size: 13px;
                            background-color: #121212;
                            color: #e0e0e0;
                            margin: 0;
                            padding: 0;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                            width: 100vw;
                        }
                        
                        p {
                            margin: 0;
                            margin-bottom: 3px;
                        }

                        .chat-container {
                            width: 100%;
                            height: 100%;
                            background-color: #1e1e1e;
                            overflow: hidden;
                            display: flex;
                            flex-direction: column;
                        }
        
                        .chat-messages {
                            padding: 20px;
                            flex: 1;
                            overflow-y: auto;
                            width: 100%;
                            box-sizing: border-box;
                        }

                        .message {
                            margin-bottom: 15px;
                            display: flex;
                            flex-direction: column;
                            position: relative;
                        }

                        .message-content {
                            max-width: 70%;
                            padding: 12px;
                            border-radius: 18px;
                            word-wrap: break-word;
                            position: relative;
                        }

                        .sent {
                            align-items: flex-end;
                        }

                        .sent .message-content {
                            background-color: #108392;
                            color: white;
                            border-top-right-radius: 5px;
                        }

                        .copy-button {
                            background-color: rgba(255, 255, 255, 0.2);
                            color: white;
                            border: none;
                            border-radius: 4px;
                            padding: 4px 4px;
                            font-size: 12px;
                            cursor: pointer;
                            display: flex;
                            align-items: center;
                            transition: background-color 0.2s;
                            position: absolute;
                            top: 8px;
                            right: 8px;
                            opacity: 0;
                        }

                        .message-content:hover .copy-button {
                            opacity: 1;
                        }

                        .copy-button:hover {
                            background-color: rgba(255, 255, 255, 0.3);
                        }

                        .copy-icon {
                            width: 14px;
                            height: 14px;
                        }

                        .received {
                            align-items: flex-start;
                        }
                    
                        .received .message-content {
                            background-color: #2c2c2c;
                            color: #e0e0e0;
                            border-top-left-radius: 5px;
                            padding-right: 33px;
                        }

                        code {
                            background-color: #272727;
                            color: #e0e0e0;
                            padding: 2px 5px;
                            border-radius: 4px;
                            font-family: Consolas, Monaco, 'Andale Mono', monospace;
                            font-size: 0.9em;
                            color: #20B8CD;
                        }
        
                        pre {
                            background-color: #2a2a2a;
                            border-radius: 5px;
                            padding: 10px;
                            overflow-x: auto;
                            margin: 0;
                        }

                        ::-webkit-scrollbar {
                            width: 8px;
                        }

                        ::-webkit-scrollbar-track {
                            background: #1e1e1e;
                        }

                        ::-webkit-scrollbar-thumb {
                            background: #444;
                            border-radius: 4px;
                        }

                        ::-webkit-scrollbar-thumb:hover {
                            background: #555;
                        }
                    </style>
                </head>
                <body>
                    <div class=""chat-container"">
                        <div class=""chat-messages"">
                            $CHAT-MESSAGES$
                        </div>
                    </div>
                    <script>
                        window.onload = function() {
                            document.getElementById('last-message').scrollIntoView();
                        };

                        function copyToClipboard(element, button) {
                            let content = element.innerText || element.textContent;
                            content = content.trim();
                            const textArea = document.createElement(""textarea"");
                            textArea.value = content;
                            document.body.appendChild(textArea);
                            textArea.focus();
                            textArea.select();
                            try {
                                document.execCommand('copy');
                                const buttonOriginalHtml = button.innerHTML;
                                button.innerHTML = `
                                    <svg class=""copy-icon"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                                        <path d=""M20 6L9 17L4 12"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""/>
                                    </svg>
                                `;
                                setTimeout(() => {
                                    button.innerHTML = buttonOriginalHtml;
                                }, 500);
                            } catch (err) {
                                console.error('Failed to copy text: ', err);
                                alert('Failed to copy text to clipboard');
                            }
                            document.body.removeChild(textArea);
                        }
                    </script>
                </body>
                </html>
            ";
        }
    }
}
