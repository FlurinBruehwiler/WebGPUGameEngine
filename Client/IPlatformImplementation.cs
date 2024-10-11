using GameEngine.WebGPU;

namespace GameEngine;

public interface IPlatformImplementation
{
    Task<Texture> LoadTexture(string name);
    Task<Stream> LoadStream(string name);
    IGPUTextureView CreateView();
}