using Microsoft.Win32;
using SFDCreator.Core.Settings;

namespace SFDCreator.IO.Settings;

public sealed class RegistrySettingsStore : ISettingsStore
{
    private const string KeyPath = @"Software\SANDEFJORD DEVELOPMENT\SFD Creator";

    public ApplicationSettings Load()
    {
        using var key = Registry.CurrentUser.OpenSubKey(KeyPath);
        if (key is null)
        {
            return new ApplicationSettings();
        }

        var defaults = new ApplicationSettings();

        return new ApplicationSettings
        {
            WindowX = (int)key.GetValue(nameof(ApplicationSettings.WindowX), defaults.WindowX)!,
            WindowY = (int)key.GetValue(nameof(ApplicationSettings.WindowY), defaults.WindowY)!,
            WindowWidth = (int)key.GetValue(nameof(ApplicationSettings.WindowWidth), defaults.WindowWidth)!,
            WindowHeight = (int)key.GetValue(nameof(ApplicationSettings.WindowHeight), defaults.WindowHeight)!,
            WindowMaximized = (int)key.GetValue(nameof(ApplicationSettings.WindowMaximized), 0)! != 0,
        };
    }

    public void Save(ApplicationSettings settings)
    {
        using var key = Registry.CurrentUser.CreateSubKey(KeyPath);

        key.SetValue(nameof(ApplicationSettings.WindowX), settings.WindowX, RegistryValueKind.DWord);
        key.SetValue(nameof(ApplicationSettings.WindowY), settings.WindowY, RegistryValueKind.DWord);
        key.SetValue(nameof(ApplicationSettings.WindowWidth), settings.WindowWidth, RegistryValueKind.DWord);
        key.SetValue(nameof(ApplicationSettings.WindowHeight), settings.WindowHeight, RegistryValueKind.DWord);
        key.SetValue(nameof(ApplicationSettings.WindowMaximized), settings.WindowMaximized ? 1 : 0, RegistryValueKind.DWord);
    }
}
