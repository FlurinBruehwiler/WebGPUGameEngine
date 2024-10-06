using System.Numerics;
using GameEngine.WebGPU;

namespace GameEngine;

public struct Vertex
{
    public required Vector3 Position;
    public required Vector2 Texcoord;
}

public class Model : IDisposable
{
    public required Vertex[] Vertices;
    public GPUBuffer? GpuBuffer;
    public Texture? Texture;
    public GPUBindGroup? TextureBindGroup;

    public void Dispose()
    {
        GpuBuffer?.Destory();
    }
}