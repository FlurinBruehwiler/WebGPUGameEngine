using System.Numerics;
using Game.WebGPU;

namespace Game;

public class Model : IDisposable
{
    public required Vector3[] Vertices;
    public GPUBuffer? GpuBuffer;

    public void Dispose()
    {
        GpuBuffer?.Destory();
    }
}