using SharpGLTF.Schema2;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Silk.NET.Maths;
using System.Runtime.InteropServices;
using Silk.NET.WebGPU.Extensions.Disposal;
using Silk.NET.WebGPU;
using Silk.NET.Core.Native;

namespace SoftTouch.Graphics.WGPU;

public unsafe class GraphicsState
{
    static GraphicsState gfxState = null!;
    public static GraphicsState GetOrCreate(IWindow? window = null)
    {
        if (gfxState is not null)
            return gfxState;
        gfxState = new GraphicsState(window);
        return gfxState;
    }

    public Silk.NET.WebGPU.WebGPU Api { get; private set; } = null!;
    public WebGPUDisposal Disposal { get; private set; } = null!;
    public Adapter Adapter { get; private set; }
    public Instance Instance { get; private set; }
    public Surface Surface { get; private set; }
    public Device Device { get; private set; }


    GraphicsState(IWindow? window)
    {
        Api = Silk.NET.WebGPU.WebGPU.GetApi();
        Disposal = new(Api);
        var cs = new ChainedStruct();

        if (window is not null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cs.SType = SType.SurfaceDescriptorFromWindowsHwnd;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cs.SType = SType.SurfaceDescriptorFromXlibWindow;
            }
            var desc = new InstanceDescriptor() { NextInChain = &cs };
            Instance = new(Api.CreateInstance(desc));

            Surface surface = new(window.CreateWebGPUSurface(Api, Instance));
            {
                var requestAdapterOptions = new RequestAdapterOptions
                {
                    CompatibleSurface = surface.Handle
                };

                Api.InstanceRequestAdapter
                (
                    Instance,
                    requestAdapterOptions,
                    new PfnRequestAdapterCallback((_, adapter1, _, _) => Adapter = new(adapter1)),
                    null
                );

                Console.WriteLine($"Got adapter {Adapter:X}");
            }
        }
        else
        {
            cs.SType = SType.Force32;

            var desc = new InstanceDescriptor()
            {
                NextInChain = &cs
            };
            Instance = new(Api.CreateInstance(desc));

            var options = new RequestAdapterOptions
            {
                CompatibleSurface = null,
                PowerPreference = PowerPreference.Undefined
            };
            Api.InstanceRequestAdapter(Instance, &options, new((_, ad, _, _) => Adapter = new(ad)), null);
        }
        {


            var deviceDescriptor = new DeviceDescriptor
            {
                RequiredLimits = null,
                DefaultQueue = new QueueDescriptor(),
                RequiredFeatures = null
            };

            Api.AdapterRequestDevice
            (
                Adapter,
                deviceDescriptor,
                new PfnRequestDeviceCallback((_, device1, _, _) => Device = new(device1)),
                null
            );

            Console.WriteLine($"Got device {Device:X}");
        } //Get device
        var features = stackalloc FeatureName[100];
        Api.DeviceEnumerateFeatures(Device, features);
        Api.DeviceSetUncapturedErrorCallback(Device, new PfnErrorCallback(UncapturedError), null);
        Api.DeviceSetDeviceLostCallback(Device, new PfnDeviceLostCallback(DeviceLost), null);

    }
    private static void DeviceLost(DeviceLostReason arg0, byte* arg1, void* arg2)
    {
        Console.WriteLine($"Device lost! Reason: {arg0} Message: {SilkMarshal.PtrToString((nint)arg1)}");
    }
    private static void UncapturedError(ErrorType arg0, byte* arg1, void* arg2)
    {
        Console.WriteLine($"{arg0}: {SilkMarshal.PtrToString((nint)arg1)}");
    }

    public void Load(IWindow window)
    {
    }
}