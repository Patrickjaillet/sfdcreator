namespace SFDCreator.Rendering.Shaders;

public sealed class NagaShaderCompiler : IShaderCompiler
{
    public ShaderCompilationResult Compile(ShaderCompilationRequest request)
    {
        throw new NotSupportedException(
            "Naga shader translation is not wired up yet: it requires native interop with the naga Rust crate, which has no official managed bindings. Implement IShaderCompiler.Compile via P/Invoke against a compiled naga cdylib before using this backend.");
    }
}
