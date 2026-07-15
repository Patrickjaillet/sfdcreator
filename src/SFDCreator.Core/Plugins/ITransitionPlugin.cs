namespace SFDCreator.Core.Plugins;

public interface ITransitionPlugin : IPlugin
{
    TimeSpan DefaultDuration { get; }
}
