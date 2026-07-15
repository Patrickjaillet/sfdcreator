namespace SFDCreator.Rendering.Backend;

public static class GraphicsDeviceFactory
{
    public static IGraphicsDevice Create(GraphicsBackendKind backend, nint hwnd, GraphicsDeviceOptions options)
    {
        return backend switch
        {
            GraphicsBackendKind.OpenGL => OpenGL.OpenGlContextFactory.Create(hwnd, options),
            GraphicsBackendKind.Vulkan => throw new NotSupportedException(
                "The Vulkan backend is not implemented yet: it requires a from-scratch native device/swapchain bootstrap. Only OpenGL is wired up so far."),
            GraphicsBackendKind.Direct3D11 => throw new NotSupportedException(
                "The Direct3D11 backend is not implemented yet: it requires a from-scratch native device/swapchain bootstrap. Only OpenGL is wired up so far."),
            GraphicsBackendKind.Direct3D12 => throw new NotSupportedException(
                "The Direct3D12 backend is not implemented yet: it requires a from-scratch native device/swapchain bootstrap. Only OpenGL is wired up so far."),
            _ => throw new ArgumentOutOfRangeException(nameof(backend), backend, null),
        };
    }
}
