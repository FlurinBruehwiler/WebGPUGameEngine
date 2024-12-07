using System.Runtime.InteropServices;
using System.Text;
using Client.WebGPU;
using Silk.NET.WebGPU;
using AddressMode = Silk.NET.WebGPU.AddressMode;
using BindGroupDescriptor = Client.WebGPU.BindGroupDescriptor;
using BindGroupEntry = Silk.NET.WebGPU.BindGroupEntry;
using BindGroupLayoutDescriptor = Client.WebGPU.BindGroupLayoutDescriptor;
using BufferBindingLayout = Silk.NET.WebGPU.BufferBindingLayout;
using FilterMode = Silk.NET.WebGPU.FilterMode;
using PipelineLayoutDescriptor = Client.WebGPU.PipelineLayoutDescriptor;
using RenderPipelineDescriptor = Client.WebGPU.RenderPipelineDescriptor;
using SamplerBindingLayout = Silk.NET.WebGPU.SamplerBindingLayout;
using SamplerDescriptor = Client.WebGPU.SamplerDescriptor;
using ShaderModuleDescriptor = Client.WebGPU.ShaderModuleDescriptor;
using TextureBindingLayout = Silk.NET.WebGPU.TextureBindingLayout;
using TextureDescriptor = Client.WebGPU.TextureDescriptor;
using TextureFormat = Silk.NET.WebGPU.TextureFormat;
using VertexAttribute = Silk.NET.WebGPU.VertexAttribute;

namespace Desktop.WebGPU;

public unsafe class GPUDevice : IGPUDevice
{
    public required Device* Device;

    public IGPUQueue Queue
    {
        get
        {
            var queue = GPU.API.DeviceGetQueue(Device);
            if (cachedGPUQueue == null || queue != cachedGPUQueue.Queue)
            {
                return cachedGPUQueue = new GPUQueue
                {
                    Queue = queue
                };
            }

            return cachedGPUQueue;
        }
    }
    private GPUQueue? cachedGPUQueue;

    public IGPUCommandEncoder CreateCommandEncoder()
    {
        var descriptor = new CommandEncoderDescriptor();

        return new GPUCommandEncoder
        {
            CommandEncoder = GPU.API.DeviceCreateCommandEncoder(Device, in descriptor)
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
            Buffer = GPU.API.DeviceCreateBuffer(Device, in bufferDescriptor),
            Size = descriptor.Size
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

        var depthStencilState = new DepthStencilState
        {
            DepthCompare = (Silk.NET.WebGPU.CompareFunction)descriptor.DepthStencil.DepthCompare,
            Format = (TextureFormat)descriptor.DepthStencil.Format,
            DepthWriteEnabled = descriptor.DepthStencil.DepthWriteEnabled,
        };

        var nativeDescriptor = new Silk.NET.WebGPU.RenderPipelineDescriptor
        {
            Vertex = new VertexState
            {
                Module = ((GPUShaderModule)descriptor.Vertex.Module).ShaderModule,
                Buffers = bufferLayouts,
                BufferCount = (nuint)descriptor.Vertex.Buffers.Length,
                EntryPoint = descriptor.Vertex.EntryPoint.ToPtr() //this probably only works if it is a string literal
            },
            Fragment = &fragmentState,
            Layout = ((GPUPipelineLayout)descriptor.Layout).PipelineLayout,
            Primitive = new PrimitiveState
            {
                Topology = (Silk.NET.WebGPU.PrimitiveTopology)descriptor.Primitive.Topology
            },
            DepthStencil = &depthStencilState
        };

        return new GPURenderPipeline
        {
            RenderPipeline = GPU.API.DeviceCreateRenderPipeline(Device, in nativeDescriptor)
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
        var stringAsBytes = Encoding.UTF8.GetBytes(descriptor.Code);
        Array.Resize(ref stringAsBytes, stringAsBytes.Length + 1);
        IntPtr ptr = Marshal.AllocHGlobal(stringAsBytes.Length);
        Marshal.Copy(stringAsBytes, 0, ptr, stringAsBytes.Length);

        var wgslDescriptor = new ShaderModuleWGSLDescriptor
        {
            Code = (byte*)ptr,
            Chain = new ChainedStruct
            {
                Next = null,
                SType = SType.ShaderModuleWgslDescriptor
            }
        };

        var shaderModuleDescriptor = new Silk.NET.WebGPU.ShaderModuleDescriptor
        {
            NextInChain = &wgslDescriptor.Chain
        };

        return new GPUShaderModule
        {
            ShaderModule = GPU.API.DeviceCreateShaderModule(Device, in shaderModuleDescriptor)
        };
    }

    public IGPUTexture CreateTexture(TextureDescriptor descriptor)
    {
        var size = new Extent3D
        {
            Width = (uint)descriptor.Size.Width,
            Height = (uint)descriptor.Size.Height,
            DepthOrArrayLayers = (uint)descriptor.Size.DepthOrArrayLayers
        };

        var textureDescriptor = new Silk.NET.WebGPU.TextureDescriptor
        {
            Format = (TextureFormat)descriptor.Format,
            Size = size,
            Usage = (TextureUsage)descriptor.Usage,
            SampleCount = 1,
            Dimension = TextureDimension.Dimension2D,
        };

        return new GPUTexture
        {
            Texture = GPU.API.DeviceCreateTexture(Device, in textureDescriptor)
        };
    }

    public IGPUPipelineLayout CreatePipelineLayout(PipelineLayoutDescriptor descriptor)
    {
        var bindGroupLayouts = stackalloc BindGroupLayout*[descriptor.BindGroupLayouts.Length];
        for (int i = 0; i < descriptor.BindGroupLayouts.Length; i++)
        {
            bindGroupLayouts[i] = ((GPUBindGroupLayout)descriptor.BindGroupLayouts[i]).BindGroupLayout;
        }

        var pipelineLayoutDescriptor = new Silk.NET.WebGPU.PipelineLayoutDescriptor
        {
            BindGroupLayouts = bindGroupLayouts,
            BindGroupLayoutCount = (nuint)descriptor.BindGroupLayouts.Length
        };

        return new GPUPipelineLayout
        {
            PipelineLayout = GPU.API.DeviceCreatePipelineLayout(Device, in pipelineLayoutDescriptor)
        };
    }

    public IGPUSampler CreateSampler(SamplerDescriptor descriptor)
    {
        var samplerDescriptor = new Silk.NET.WebGPU.SamplerDescriptor
        {
            AddressModeU = (AddressMode)(descriptor.AddressModeU ?? Client.WebGPU.AddressMode.ClampToEdge),
            AddressModeV = (AddressMode)(descriptor.AddressModeV ?? Client.WebGPU.AddressMode.ClampToEdge),
            AddressModeW = (AddressMode)(descriptor.AddressModeW ?? Client.WebGPU.AddressMode.ClampToEdge),
            MagFilter = (FilterMode)(descriptor.MagFilter ?? Client.WebGPU.FilterMode.Linear)
        };

        return new GPUSampler
        {
            Sampler = GPU.API.DeviceCreateSampler(Device, in samplerDescriptor)
        };
    }
}