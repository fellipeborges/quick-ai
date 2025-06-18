using Microsoft.Win32;
using quick_ai.Constants;
using quick_ai.Model;

namespace quick_ai.Service
{
    internal static class SettingsService
    {
        public static Settings GetSettings() =>
            new()
            {
                WindowTop = ReadFromRegistry(SettingsConstants.SETTING_WINDOW_TOP) ?? "100",
                WindowLeft = ReadFromRegistry(SettingsConstants.SETTING_WINDOW_LEFT) ?? "100",
                WindowHeight = ReadFromRegistry(SettingsConstants.SETTING_WINDOW_HEIGHT) ?? "600",
                WindowWidth = ReadFromRegistry(SettingsConstants.SETTING_WINDOW_WIDTH) ?? "800",
                WindowMaximized = ReadFromRegistry(SettingsConstants.SETTING_WINDOW_MAXIMIZED) ?? "false",
                AzureOpenAIEndpoint = ReadFromRegistry(SettingsConstants.SETTING_AZURE_OPENAI_ENDPOINT) ?? string.Empty,
                AzureOpenAIKey = ReadFromRegistry(SettingsConstants.SETTING_AZURE_OPENAI_KEY) ?? string.Empty,
                AzureOpenAIDeployment = ReadFromRegistry(SettingsConstants.SETTING_AZURE_OPENAI_DEPLOYMENT) ?? string.Empty,
            };

        public static void SaveGeneralSettings(string azureOpenAIEndpoint, string azureOpenAIKey, string azureOpenAIDeployment)
        {
            WriteToRegistry(SettingsConstants.SETTING_AZURE_OPENAI_ENDPOINT, azureOpenAIEndpoint);
            WriteToRegistry(SettingsConstants.SETTING_AZURE_OPENAI_KEY, azureOpenAIKey);
            WriteToRegistry(SettingsConstants.SETTING_AZURE_OPENAI_DEPLOYMENT, azureOpenAIDeployment);
        }

        public static void SaveWindowSettings(string top, string left, string height, string width, bool maximized)
        {
            WriteToRegistry(SettingsConstants.SETTING_WINDOW_TOP, top);
            WriteToRegistry(SettingsConstants.SETTING_WINDOW_LEFT, left);
            WriteToRegistry(SettingsConstants.SETTING_WINDOW_HEIGHT, height);
            WriteToRegistry(SettingsConstants.SETTING_WINDOW_WIDTH, width);
            WriteToRegistry(SettingsConstants.SETTING_WINDOW_MAXIMIZED, maximized.ToString());
        }

        private static void WriteToRegistry(string keyName, string keyValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{SettingsConstants.SETTING_APP_NAME}");
            if (key != null)
            {
                key.SetValue(keyName, keyValue, RegistryValueKind.String);
                key.Close();
            }
        }

        public static string? ReadFromRegistry(string keyName) =>
            Registry
                .CurrentUser
                .OpenSubKey($"SOFTWARE\\{SettingsConstants.SETTING_APP_NAME}")
                ?.GetValue(keyName)
                ?.ToString();

    }
}
