using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Resources;

public sealed class ShaderProgram : IDisposable
{
    private readonly GL _gl;
    private readonly Dictionary<string, int> _uniformLocations = new();

    public uint Handle { get; }

    public ShaderProgram(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;

        var vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        var fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

        Handle = _gl.CreateProgram();
        _gl.AttachShader(Handle, vertexShader);
        _gl.AttachShader(Handle, fragmentShader);
        _gl.LinkProgram(Handle);

        _gl.GetProgram(Handle, GLEnum.LinkStatus, out var linkStatus);
        if (linkStatus == 0)
        {
            var log = _gl.GetProgramInfoLog(Handle);
            throw new InvalidOperationException($"Failed to link shader program: {log}");
        }

        _gl.DetachShader(Handle, vertexShader);
        _gl.DetachShader(Handle, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }

    public void Use() => _gl.UseProgram(Handle);

    public void SetUniform(string name, float value) => _gl.Uniform1(Location(name), value);

    public void SetUniform(string name, int value) => _gl.Uniform1(Location(name), value);

    public void SetUniform(string name, Vector2 value) => _gl.Uniform2(Location(name), value);

    public void SetUniform(string name, Vector3 value) => _gl.Uniform3(Location(name), value);

    public void SetUniform(string name, Matrix4x4 value) =>
        _gl.UniformMatrix4(Location(name), false, MemoryMarshal.CreateReadOnlySpan(ref value.M11, 16));

    private int Location(string name)
    {
        if (_uniformLocations.TryGetValue(name, out var cached))
        {
            return cached;
        }

        var location = _gl.GetUniformLocation(Handle, name);
        _uniformLocations[name] = location;
        return location;
    }

    private uint CompileShader(ShaderType type, string source)
    {
        var shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, GLEnum.CompileStatus, out var status);
        if (status == 0)
        {
            var log = _gl.GetShaderInfoLog(shader);
            throw new InvalidOperationException($"Failed to compile {type} shader: {log}");
        }

        return shader;
    }

    public void Dispose() => _gl.DeleteProgram(Handle);
}
