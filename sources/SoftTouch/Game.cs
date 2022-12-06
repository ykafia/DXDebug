using System;
using WGPU.NET;
using System.Diagnostics;
using Silk.NET.GLFW;
using System.Runtime.InteropServices;
using System.IO;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.PixelFormats;
using SoftTouch.ECS;
using Silk.NET.Windowing;
using SoftTouch.Graphics.WGPU;
using SoftTouch.Assets;
using Zio.FileSystems;
using SoftTouch.Rendering;
using System.Threading.Tasks;

namespace SoftTouch;

public class Game : IGame
{
    IWindow window;
    WGPUGraphics Graphics = new();
    GameWorld world;
    AssetManager assetManager;

    public Game()
    {
        window = Window.Create(WindowOptions.Default);
        window.Initialize();
        OnLoad();

        world = new();
        var fs = new PhysicalFileSystem();
        var ss = new SubFileSystem(fs, fs.ConvertPathFromInternal("../../assets"));
        assetManager = new(ss);
        world.SetResource(assetManager);
        world.SetResource(Graphics);
        world.AddStartup<Processors.Startup>();
    }

    public void Run()
    {
        while (!window.IsClosing)
        {
            Task.WaitAll(
                Task.Run(world.Update),
                Task.Run(world.Render)
            );
            world.Extract();
        }
    }

    public void OnLoad()
    {
        Graphics.LoadWindow(window);
    }
}
