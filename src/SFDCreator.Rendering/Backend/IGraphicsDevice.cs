namespace SFDCreator.Rendering.Backend;

public interface IGraphicsDevice : IDisposable
{
    GraphicsBackendKind Backend { get; }

    void MakeCurrent();

    void SwapBuffers();
}
