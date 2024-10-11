using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUShaderModule : IGPUShaderModule
{
    public required ShaderModule* ShaderModule;
}
