namespace SFDCreator.Rendering.Shaders;

public sealed class ShaderCompilationRequest
{
    public required string Source { get; init; }

    public required ShaderSourceLanguage SourceLanguage { get; init; }

    public required ShaderStage Stage { get; init; }

    public required ShaderSourceLanguage TargetLanguage { get; init; }

    public string EntryPoint { get; init; } = "main";
}
