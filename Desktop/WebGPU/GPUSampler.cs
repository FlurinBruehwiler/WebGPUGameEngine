using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUSampler : IGPUSampler
{
    public required Sampler* Sampler;
}