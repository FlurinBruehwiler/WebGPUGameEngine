using System.Runtime.InteropServices;
using GameEngine.WebGPU;
using Silk.NET.WebGPU;
using WasmTestCSharp.WebGPU;
using BindGroupDescriptor = GameEngine.WebGPU.BindGroupDescriptor;
using BindGroupEntry = Silk.NET.WebGPU.BindGroupEntry;
using BindGroupLayoutDescriptor = GameEngine.WebGPU.BindGroupLayoutDescriptor;
using BufferBindingLayout = Silk.NET.WebGPU.BufferBindingLayout;
using PipelineLayoutDescriptor = GameEngine.WebGPU.PipelineLayoutDescriptor;
using RenderPipelineDescriptor = GameEngine.WebGPU.RenderPipelineDescriptor;
using SamplerBindingLayout = Silk.NET.WebGPU.SamplerBindingLayout;
using SamplerDescriptor = GameEngine.WebGPU.SamplerDescriptor;
using ShaderModuleDescriptor = GameEngine.WebGPU.ShaderModuleDescriptor;
using TextureBindingLayout = Silk.NET.WebGPU.TextureBindingLayout;
using TextureDescriptor = GameEngine.WebGPU.TextureDescriptor;
using TextureFormat = Silk.NET.WebGPU.TextureFormat;
using VertexAttribute = Silk.NET.WebGPU.VertexAttribute;

namespace Desktop.WebGPU;

public unsafe class GPUDevice : IGPUDevice
{
    public required Device* Device;

    public IGPUQueue Queue { get; }

    public IGPUCommandEncoder CreateCommandEncoder()
    {
        var descriptor = new CommandEncoderDescriptor();

        return new GPUCommandEncoder
        {
            CommandEncoder = GPU.API.DeviceCreateCommandEncoder(Device, descriptor)
        };
    }

    public IGPUBuffer CreateBuffer(CreateBufferDescriptor descriptor)
    {
        var bufferDescriptor = new BufferDescriptor
        {
            Size = (ulong)descriptor.Size,
            Usage = (BufferUsage)descriptor.Usage
        };

        return new GPUBuffer
        {
            Buffer = GPU.API.DeviceCreateBuffer(Device, in bufferDescriptor)
        };
    }

    public IGPURenderPipeline CreateRenderPipeline(RenderPipelineDescriptor descriptor)
    {
        var bufferLayouts = stackalloc VertexBufferLayout[descriptor.Vertex.Buffers.Length];
        for (var i = 0; i < descriptor.Vertex.Buffers.Length; i++)
        {
            var bufferDescriptor = descriptor.Vertex.Buffers[i];

            // ReSharper disable once StackAllocInsideLoop
            var attributes = stackalloc VertexAttribute[bufferDescriptor.Attributes.Length];
            for (var j = 0; j < bufferDescriptor.Attributes.Length; j++)
            {
                var vertexAttribute = bufferDescriptor.Attributes[j];

                attributes[j] = new VertexAttribute
                {
                    Format = (Silk.NET.WebGPU.VertexFormat)vertexAttribute.Format,
                    Offset = (ulong)vertexAttribute.Offset,
                    ShaderLocation = (uint)vertexAttribute.ShaderLocation
                };
            }

            bufferLayouts[i] = new VertexBufferLayout
            {
                Attributes = attributes,
                AttributeCount = (nuint)bufferDescriptor.Attributes.Length,
                ArrayStride = (ulong)bufferDescriptor.ArrayStride,
                StepMode = (Silk.NET.WebGPU.VertexStepMode)bufferDescriptor.StepMode
            };
        }

        var targets = stackalloc ColorTargetState[descriptor.Fragment.Targets.Length];
        for (var i = 0; i < descriptor.Fragment.Targets.Length; i++)
        {
            var target = descriptor.Fragment.Targets[i];

            targets[i] = new ColorTargetState
            {
                Format = (TextureFormat)target.Format
            };
        }

        var fragmentState = new FragmentState
        {
            Module = ((GPUShaderModule)descriptor.Fragment.Module).ShaderModule,
            EntryPoint = descriptor.Fragment.EntryPoint.ToPtr(),
            Targets = targets,
            TargetCount = (nuint)descriptor.Fragment.Targets.Length,
        };
        IntPtr fragmentPtr = IntPtr.Zero;
        Marshal.StructureToPtr(fragmentState, fragmentPtr, false);

        var depthStencilState = new DepthStencilState
        {
            DepthCompare = (Silk.NET.WebGPU.CompareFunction)descriptor.DepthStencil.DepthCompare,
            Format = (TextureFormat)descriptor.DepthStencil.Format,
            DepthWriteEnabled = descriptor.DepthStencil.DepthWriteEnabled
        };
        IntPtr depthStencilPtr = IntPtr.Zero;
        Marshal.StructureToPtr(depthStencilState, depthStencilPtr, false);

        var nativeDescriptor = new Silk.NET.WebGPU.RenderPipelineDescriptor
        {
            Vertex = new VertexState
            {
                Module = ((GPUShaderModule)descriptor.Vertex.Module).ShaderModule,
                Buffers = bufferLayouts,
                BufferCount = (nuint)descriptor.Vertex.Buffers.Length,
                EntryPoint = descriptor.Vertex.EntryPoint.ToPtr() //this probably only works if it is a string literal
            },
            Fragment = (FragmentState*)fragmentPtr,
            Layout = ((GPUPipelineLayout)descriptor.Layout).PipelineLayout,
            Primitive = new PrimitiveState
            {
                Topology = (Silk.NET.WebGPU.PrimitiveTopology)descriptor.Primitive.Topology
            },
            DepthStencil = (DepthStencilState*)depthStencilPtr
        };

        return new GPURenderPipeline
        {
            RenderPipeline = GPU.API.DeviceCreateRenderPipeline(Device, nativeDescriptor)
        };
    }

    public IGPUBindGroup CreateBindGroup(BindGroupDescriptor descriptor)
    {
        var entries = stackalloc BindGroupEntry[descriptor.Entries.Length];
        for (var i = 0; i < descriptor.Entries.Length; i++)
        {
            var entry = descriptor.Entries[i];

            var newEntry = new BindGroupEntry
            {
                Binding = (uint)entry.Binding
            };

            if (entry.Resource is BufferBinding bufferBinding)
            {
                newEntry.Buffer = ((GPUBuffer)bufferBinding.Buffer).Buffer;
                newEntry.Size = (ulong)(bufferBinding.Size ?? 0);
                newEntry.Offset = (ulong)(bufferBinding.Offset ?? 0);
            }
            else if (entry.Resource is GPUSampler sampler)
            {
                newEntry.Sampler = sampler.Sampler;
            }
            else if (entry.Resource is GPUTextureView textureView)
            {
                newEntry.TextureView = textureView.TextureView;
            }

            entries[i] = newEntry;
        }

        var bindGroupDescriptor = new Silk.NET.WebGPU.BindGroupDescriptor
        {
            Layout = ((GPUBindGroupLayout)descriptor.Layout).BindGroupLayout,
            Entries = entries
        };

        return new GPUBindGroup
        {
            BindGroup = GPU.API.DeviceCreateBindGroup(Device, in bindGroupDescriptor)
        };
    }

    public IGPUBindGroupLayout CreateBindGroupLayout(BindGroupLayoutDescriptor descriptor)
    {
        var entries = stackalloc BindGroupLayoutEntry[descriptor.Entries.Length];
        for (var i = 0; i < descriptor.Entries.Length; i++)
        {
            var entry = descriptor.Entries[i];

            var newEntry = new BindGroupLayoutEntry
            {
                Binding = (uint)entry.Binding,
                Visibility = (ShaderStage)entry.Visibility,
            };

            if (entry.Buffer != null)
            {
                newEntry.Buffer = new BufferBindingLayout
                {
                    HasDynamicOffset = entry.Buffer.HasDynamicOffset
                };
            }

            if (entry.Texture != null)
            {
                newEntry.Texture = new TextureBindingLayout();
            }

            if (entry.Sampler != null)
            {
                newEntry.Sampler = new SamplerBindingLayout();
            }

            entries[i] = newEntry;
        }

        var bindGroupLayoutDescriptor = new Silk.NET.WebGPU.BindGroupLayoutDescriptor
        {
            Entries = entries
        };

        return new GPUBindGroupLayout
        {
            BindGroupLayout = GPU.API.DeviceCreateBindGroupLayout(Device, in bindGroupLayoutDescriptor)
        };
    }

    public IGPUShaderModule CreateShaderModule(ShaderModuleDescriptor descriptor)
    {
        throw new NotImplementedException();
    }

    public IGPUTexture CreateTexture(TextureDescriptor descriptor)
    {
        throw new NotImplementedException();
    }

    public IGPUPipelineLayout CreatePipelineLayout(PipelineLayoutDescriptor descriptor)
    {
        throw new NotImplementedException();
    }

    public IGPUSampler CreateSampler(SamplerDescriptor descriptor)
    {
        throw new NotImplementedException();
    }
}