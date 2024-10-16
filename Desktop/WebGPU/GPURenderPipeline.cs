﻿using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPipeline
/// </summary>
public unsafe class GPURenderPipeline : IGPURenderPipeline
{
    public required RenderPipeline* RenderPipeline;

    public IGPUBindGroupLayout GetBindGroupLayout(int index)
    {
        return new GPUBindGroupLayout
        {
            BindGroupLayout = GPU.API.RenderPipelineGetBindGroupLayout(RenderPipeline, (uint)index)
        };
    }
}
