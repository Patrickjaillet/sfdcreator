namespace SFDCreator.Rendering.Shaders;

public interface IShaderCompiler
{
    ShaderCompilationResult Compile(ShaderCompilationRequest request);
}
