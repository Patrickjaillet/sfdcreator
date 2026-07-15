using System.Text.Json;
using SFDCreator.Core.Settings;

namespace SFDCreator.IO.Settings;

public sealed class JsonFileSettingsStore : ISettingsStore
{
    private readonly string _filePath;

    public JsonFileSettingsStore()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SFD Creator", "settings.json"))
    {
    }

    public JsonFileSettingsStore(string filePath)
    {
        _filePath = filePath;
    }

    public ApplicationSettings Load()
    {
        if (!File.Exists(_filePath))
        {
            return new ApplicationSettings();
        }

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<ApplicationSettings>(json) ?? new ApplicationSettings();
    }

    public void Save(ApplicationSettings settings)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
