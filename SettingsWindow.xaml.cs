using quick_ai.Model;
using quick_ai.Service;
using System.Windows;
using System.Windows.Input;

namespace quick_ai
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
            txtAzureOpenAIEndpoint.Focus();
        }

        private void LoadSettings()
        {
            Settings settings = SettingsService.GetSettings();
            txtAzureOpenAIEndpoint.Text = settings.AzureOpenAIEndpoint;
            txtAzureOpenAIKey.Text = settings.AzureOpenAIKey;
            txtAzureOpenAIDeployment.Text = settings.AzureOpenAIDeployment;
        }

        private void SaveSettings() =>
            SettingsService.SaveGeneralSettings(
                azureOpenAIEndpoint: txtAzureOpenAIEndpoint.Text.Trim(),
                azureOpenAIKey: txtAzureOpenAIKey.Text.Trim(),
                azureOpenAIDeployment: txtAzureOpenAIDeployment.Text.Trim()
            );

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
            {
                e.Handled = true;
                SaveSettings();
                Close();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void txtAzureOpenAIEndpoint_GotFocus(object sender, RoutedEventArgs e)
        {
            txtAzureOpenAIEndpoint.SelectAll();
        }

        private void txtAzureOpenAIKey_GotFocus(object sender, RoutedEventArgs e)
        {
            txtAzureOpenAIKey.SelectAll();
        }

        private void txtAzureOpenAIDeployment_GotFocus(object sender, RoutedEventArgs e)
        {
            txtAzureOpenAIDeployment.SelectAll();
        }
    }
}
