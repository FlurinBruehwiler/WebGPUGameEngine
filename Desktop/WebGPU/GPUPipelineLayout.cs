using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUPipelineLayout : IGPUPipelineLayout
{
    public required PipelineLayout* PipelineLayout { get; init; }
}