namespace SFDCreator.Core.Settings;

public interface ISettingsStore
{
    ApplicationSettings Load();

    void Save(ApplicationSettings settings);
}
