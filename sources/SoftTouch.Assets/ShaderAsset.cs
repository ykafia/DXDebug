using SoftTouch.Graphics.WebGPU;
using Zio;

namespace SoftTouch.Assets;

public class ShaderAsset : AssetItem
{

    public ShaderAsset(UPath path) : base(path)
    {
    }
    // public required string Module {get; init;}
    // public static ShaderAsset Load(in UPath path, IFileSystem fs)
    // {
    //     if(fs.FileExists(path) && path.GetExtensionWithDot() == ".wgsl")
    //         return new ShaderAsset{ Module = fs.ReadAllText(path)};
    //     else
    //         throw new Exception("Not a wgsl file");
    // }

    // public void Load(WGPUGraphics gfx)
    // {
    //     throw new NotImplementedException();
    // }

    // public void Unload()
    // {
    //     throw new NotImplementedException();
    // }
    
}