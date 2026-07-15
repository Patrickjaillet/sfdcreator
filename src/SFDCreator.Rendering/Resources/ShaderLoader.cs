using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Resources;

public static class ShaderLoader
{
    public static ShaderProgram Load(GL gl, string shaderDirectory, string vertexFile, string fragmentFile)
    {
        var vertexSource = File.ReadAllText(Path.Combine(shaderDirectory, vertexFile));
        var fragmentSource = File.ReadAllText(Path.Combine(shaderDirectory, fragmentFile));
        return new ShaderProgram(gl, vertexSource, fragmentSource);
    }
}
