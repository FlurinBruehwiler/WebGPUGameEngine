using Client.WebGPU;

namespace Client;

public interface IPlatformImplementation
{
    Task<Texture> LoadTexture(string name);
    Task<Stream> LoadStream(string name);
    IGPUTextureView CreateTextureView();
}