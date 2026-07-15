namespace SFDCreator.Core.Plugins;

public interface IGeneratorPlugin : IPlugin
{
    string OutputKind { get; }
}
