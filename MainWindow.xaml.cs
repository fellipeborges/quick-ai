using quick_ai.Constants;
using quick_ai.Model;
using quick_ai.Service;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace quick_ai
{
    public partial class MainWindow : Window
    {
        private readonly List<ChatInteration> ChatInterations = [];
        private string LastTypeUsed = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
            ChatInterationClear();
        }

        private void InitializeWindow()
        {
            Settings settings = SettingsService.GetSettings();
            Top = double.Parse(settings.WindowTop);
            Left = double.Parse(settings.WindowLeft);
            Height = double.Parse(settings.WindowHeight);
            Width = double.Parse(settings.WindowWidth);
            WindowState = bool.Parse(settings.WindowMaximized) ? WindowState.Maximized : WindowState.Normal;
            richPrompt.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(richPrompt_OnPaste));
            PromptTextAppend(System.Diagnostics.Debugger.IsAttached ? "Como listar os arquivos de um diretório utilizando Powershell?" : "");
            richPrompt.Focus();
        }

        private void StartNewPrompt()
        {
            ChatInterationClear();
            PromptTextSet("");
            richPrompt.Focus();
        }

        private void TypeSetCurrent(string type)
        {
            if (type != LastTypeUsed)
            {
                ChatInterationClear();
            }
            LastTypeUsed = type;
        }

        private async void PromptSend()
        {
            string prompt = PromptTextGet();
            if (string.IsNullOrWhiteSpace(prompt))
                return;

            string selectedType = TypeGetCurrentSelected();
            TypeSetCurrent(selectedType);
            PromptTextSet("");
            richPrompt.IsEnabled = false;
            HandlePromptButtonsEnableness();
            ChatInterationAdd(ChatConstants.CHAT_SENDER_USER, prompt);
            ChatInterationAdd(ChatConstants.CHAT_SENDER_AI, ChatInterationGetRandomThinkingMessage(), temporary: true);
            AiResponse? response = null;

            if (selectedType == TypeConstants.TYPE_GENERAL)
            {
                response = await AiService.ProcessAsync(ChatInterations);
            }
            else if (selectedType == TypeConstants.TYPE_GRAMMAR)
            {
                response = await AiLanguageService.ProcessAsync(ChatInterations);
            }

            if (response != null)
            {
                ChatInterationAdd(ChatConstants.CHAT_SENDER_AI, response.Content);
                ChatInterationAdd(ChatConstants.CHAT_SENDER_AI, response.ContentSecondary);
                if (!response.Success)
                    MessageBox.Show(response.ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            richPrompt.IsEnabled = true;
            richPrompt.Focus();
        }

        private async void SurpriseMe()
        {
            TypeSetCurrent(TypeConstants.TYPE_SURPRISE_ME);

            string[] messageTypes =
            [
                "Conte uma piada leve e rápida.",
                "Cite uma frase muito famosa de uma personalidade.",
                "Ensine rapidamente algo muito útil para o dia-a-dia mas que nem todos sabem.",
                "Conte uma curiosidade interessante, simples e rápida.",
                "Dê saudações mas de uma forma passivo agressiva.",
                "Fale uma frase filosófica que não tem resposta conhecida pela humanidade.",
                "Conte uma charada simples e rápida.",
            ];


            string prompt = messageTypes[new Random().Next(messageTypes.Length)];
            ChatInterationAdd(ChatConstants.CHAT_SENDER_USER, prompt);
            prompt += " Não inicie a resposta com 'Claro!' ou 'Com certeza!', apenas responda diretamente.";

            var chatIterations = new List<ChatInteration> {
                new() {
                    Message = prompt,
                    Position = 1,
                    Sender = ChatConstants.CHAT_SENDER_AI,
                    Temporary = false
                }
            };

            AiResponse? response = await AiService.ProcessAsync(chatIterations);
            if (response != null)
            {
                ChatInterationAdd(ChatConstants.CHAT_SENDER_AI, response.Content);
            }
        }

        private static string ChatInterationGetRandomThinkingMessage()
        {
            string[] thinkingMessages =
            [
                "💭 Pensando...",
                "🧠 Deixa eu dar uma filosofada nisso...",
                "🤔 Hmmm... isso merece uma reflexão profunda...",
                "🕵️ Investigando os mistérios do universo...",
                "🛠️ Montando a resposta... peça por peça...",
                "🦉 Consultando minha coruja da sabedoria...",
                "⏳ Carregando ideias geniais, aguarde um instante...",
                "🚀 Viajando pelo espaço das palavras... já volto...",
                "🧙‍♂️ Misturando poções de conhecimento... quase pronto...",
                "🐢 Resposta a caminho... mas às vezes sou uma tartaruga...",
                "🍵 Pausa para um chá enquanto penso nisso...",
                "🌀 Girando os neurônios... aguarde um momento...",
                "🌱 Plantando a ideia... só esperando florescer...",
                "🔍 Procurando respostas no fundo da gaveta...",
                "🛸 Chamando reforços intergalácticos... só um segundo...",
                "🎩 Tirando uma resposta da cartola... quase lá...",
                "🏗️ Construindo algo brilhante... martelo e pregos em ação..."
            ];

            return thinkingMessages[new Random().Next(thinkingMessages.Length)];
        }

        private void HandlePromptButtonsEnableness()
        {
            btnPromptSend.IsEnabled = !string.IsNullOrWhiteSpace(PromptTextGet());
            btnPromptSend.Foreground = btnPromptSend.IsEnabled ?
                                        new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(32, 184, 205)) :
                                        System.Windows.Media.Brushes.Gray;
        }

        private string PromptTextGet() => new TextRange(richPrompt.Document.ContentStart, richPrompt.Document.ContentEnd).Text.Trim();
        private void PromptTextAppend(string text) => richPrompt.CaretPosition.InsertTextInRun(text);
        private void PromptTextSet(string text)
        {
            richPrompt.Document.Blocks.Clear();
            PromptTextAppend(text);
        }

        private async void ChatInterationRefresh()
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                await webViewResult.EnsureCoreWebView2Async();
                webViewResult.NavigateToString(HtmlBuilderService.BuildHtml(ChatInterations));
            });
        }

        private void ChatInterationAdd(string sender, string message, bool temporary = false)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            ChatInterations.RemoveAll(c => c.Temporary);

            ChatInterations.Add(new ChatInteration
            {
                Sender = sender,
                Message = message,
                Temporary = temporary,
                Position = ChatInterations.Count + 1
            });

            ChatInterationRefresh();
        }

        private void ChatInterationClear()
        {
            ChatInterations.Clear();
            ChatInterationRefresh();
        }

        private string TypeGetCurrentSelected()
        {
            if (rdoTypeGeneral.IsChecked.HasValue && rdoTypeGeneral.IsChecked.Value)
                return TypeConstants.TYPE_GENERAL;
            else if (rdoTypeGrammar.IsChecked.HasValue && rdoTypeGrammar.IsChecked.Value)
                return TypeConstants.TYPE_GRAMMAR;
            else
                return TypeConstants.TYPE_GENERAL;
        }

        private void ShowSettingsWindow()
        {
            SettingsWindow settingsWindow = new()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            settingsWindow.ShowDialog();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                e.Handled = true;
                rdoTypeGeneral.IsChecked = true;
            }
            else if (e.Key == Key.F2)
            {
                e.Handled = true;
                rdoTypeGrammar.IsChecked = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.N)
            {
                e.Handled = true;
                StartNewPrompt();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                WindowState = WindowState.Minimized;
            }
        }

        private void btnPromptSend_Click(object sender, RoutedEventArgs e)
        {
            PromptSend();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsService.SaveWindowSettings(Top.ToString(), Left.ToString(), Height.ToString(), Width.ToString(), (WindowState == WindowState.Maximized));
        }

        private void txtStartNewPrompt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StartNewPrompt();
        }

        private void txtSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowSettingsWindow();
        }

        private void txtSurpriseMe_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SurpriseMe();
        }

        private void richPrompt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            HandlePromptButtonsEnableness();
        }

        private void richPrompt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Control & Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true;
                PromptSend();
            }

            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PromptTextAppend("\n");
                richPrompt.CaretPosition = richPrompt.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);
                e.Handled = true;
            }
        }

        private void richPrompt_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.Text))
            {
                string? plainText = e.SourceDataObject.GetData(DataFormats.Text) as string;
                if (!string.IsNullOrEmpty(plainText))
                {
                    PromptTextAppend(plainText);
                }
            }

            e.CancelCommand();
        }
    }
}