namespace SFDCreator.Rendering.Shaders;

public sealed class ShaderCompilationResult
{
    public required bool Success { get; init; }

    public string? TranslatedSource { get; init; }

    public IReadOnlyList<string> Diagnostics { get; init; } = Array.Empty<string>();
}
