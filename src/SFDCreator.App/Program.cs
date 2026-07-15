using SFDCreator.Core.Plugins;
using SFDCreator.Core.Settings;
using SFDCreator.IO.Settings;
using SFDCreator.Win32;
using SFDCreator.Win32.Dialogs;
using SFDCreator.Win32.Docking;
using SFDCreator.Win32.Menu;
using Silk.NET.Input;

const int CommandFileOpen = 1001;
const int CommandFileSave = 1002;
const int CommandFileExit = 1003;
const int CommandHelpAbout = 2001;

var services = new ServiceRegistry();
var pluginManager = new PluginManager(services);
pluginManager.LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "Plugins"));

ISettingsStore settingsStore = new RegistrySettingsStore();
var settings = settingsStore.Load();

var menu = new NativeMenuBuilder()
    .AddTopLevel("&File")
    .AddItem("&Open...", CommandFileOpen)
    .AddItem("&Save As...", CommandFileSave)
    .AddItem("E&xit", CommandFileExit)
    .AddTopLevel("&Help")
    .AddItem("&About", CommandHelpAbout);

using var window = new Win32Window(new Win32WindowOptions
{
    Title = "SFD Creator",
    Width = settings.WindowWidth,
    Height = settings.WindowHeight,
    X = settings.WindowX >= 0 ? settings.WindowX : null,
    Y = settings.WindowY >= 0 ? settings.WindowY : null,
    Maximized = settings.WindowMaximized,
    MenuBuilder = menu,
});

services.Register<IInputContext>(window.Input);

var dockPanels = new DockPanelHost(window);

window.FilesDropped += files =>
{
    foreach (var file in files)
    {
        Console.WriteLine($"Dropped: {file}");
    }
};

window.MenuCommand += commandId =>
{
    switch (commandId)
    {
        case CommandFileOpen:
            Win32FileDialog.OpenFile(window.Handle, "Open Project", "SFD Project (*.sfdproj)|*.sfdproj|All Files (*.*)|*.*");
            break;

        case CommandFileSave:
            Win32FileDialog.SaveFile(window.Handle, "Save Project", "SFD Project (*.sfdproj)|*.sfdproj|All Files (*.*)|*.*");
            break;

        case CommandFileExit:
            window.Close();
            break;

        case CommandHelpAbout:
            NativeMessageBox.Show(window.Handle, "SFD Creator\nCopyright (c) 2026 SANDEFJORD DEVELOPMENT", "About SFD Creator");
            break;
    }
};

window.Closing += () =>
{
    var bounds = window.GetWindowBounds();
    settingsStore.Save(settings with
    {
        WindowX = bounds.X,
        WindowY = bounds.Y,
        WindowWidth = bounds.Width,
        WindowHeight = bounds.Height,
    });
};

window.Run();

pluginManager.ShutdownAll();
