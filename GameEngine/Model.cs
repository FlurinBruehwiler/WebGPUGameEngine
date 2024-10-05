using System.Numerics;
using GameEngine.WebGPU;

namespace GameEngine;

public class Model : IDisposable
{
    public required Vector3[] Vertices;
    public GPUBuffer? GpuBuffer;

    public void Dispose()
    {
        GpuBuffer?.Destory();
    }
}