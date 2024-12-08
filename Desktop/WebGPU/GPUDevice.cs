using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Client.WebGPU;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;
using AddressMode = Silk.NET.WebGPU.AddressMode;
using BindGroupDescriptor = Client.WebGPU.BindGroupDescriptor;
using BindGroupEntry = Silk.NET.WebGPU.BindGroupEntry;
using BindGroupLayoutDescriptor = Client.WebGPU.BindGroupLayoutDescriptor;
using BufferBindingLayout = Silk.NET.WebGPU.BufferBindingLayout;
using BufferBindingType = Silk.NET.WebGPU.BufferBindingType;
using FilterMode = Silk.NET.WebGPU.FilterMode;
using PipelineLayoutDescriptor = Client.WebGPU.PipelineLayoutDescriptor;
using RenderPipelineDescriptor = Client.WebGPU.RenderPipelineDescriptor;
using SamplerBindingLayout = Silk.NET.WebGPU.SamplerBindingLayout;
using SamplerBindingType = Silk.NET.WebGPU.SamplerBindingType;
using SamplerDescriptor = Client.WebGPU.SamplerDescriptor;
using ShaderModuleDescriptor = Client.WebGPU.ShaderModuleDescriptor;
using TextureBindingLayout = Silk.NET.WebGPU.TextureBindingLayout;
using TextureDescriptor = Client.WebGPU.TextureDescriptor;
using TextureFormat = Silk.NET.WebGPU.TextureFormat;
using TextureSampleType = Silk.NET.WebGPU.TextureSampleType;
using TextureViewDimension = Silk.NET.WebGPU.TextureViewDimension;
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
            var attributes = stackalloc VertexAttribute[bufferDescriptor.Attributes.Length]; //should be fine as long as there aren't too many buffers
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
            EntryPoint = (byte*) SilkMarshal.StringToPtr(descriptor.Fragment.EntryPoint),
            Targets = targets,
            TargetCount = (nuint)descriptor.Fragment.Targets.Length,
        };

        var nativeDescriptor = new Silk.NET.WebGPU.RenderPipelineDescriptor
        {
            Vertex = new VertexState
            {
                Module = ((GPUShaderModule)descriptor.Vertex.Module).ShaderModule,
                Buffers = bufferLayouts,
                BufferCount = (nuint)descriptor.Vertex.Buffers.Length,
                EntryPoint = (byte*)SilkMarshal.StringToPtr(descriptor.Vertex.EntryPoint)
            },
            Fragment = &fragmentState,
            Layout = ((GPUPipelineLayout)descriptor.Layout).PipelineLayout,
            Primitive = new PrimitiveState
            {
                Topology = (Silk.NET.WebGPU.PrimitiveTopology)descriptor.Primitive.Topology
            },
            Multisample = new MultisampleState
            {
                Count = 1,
                AlphaToCoverageEnabled = true,
                Mask = 0xFFFFFFFF
            },
            Label = (byte*)SilkMarshal.StringToPtr("MyRenderPipeline")
        };

        Console.WriteLine(new IntPtr(nativeDescriptor.Layout));

        if (descriptor.DepthStencil != null)
        {
            var depthStencilState = new DepthStencilState
            {
                DepthCompare = (Silk.NET.WebGPU.CompareFunction)descriptor.DepthStencil.Value.DepthCompare,
                Format = (TextureFormat)descriptor.DepthStencil.Value.Format,
                DepthWriteEnabled = descriptor.DepthStencil.Value.DepthWriteEnabled,
            };

            nativeDescriptor.DepthStencil = &depthStencilState;
        }

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
            Entries = entries,
            EntryCount = (nuint)descriptor.Entries.Length
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
                    HasDynamicOffset = entry.Buffer.HasDynamicOffset,
                    MinBindingSize = (ulong)entry.Buffer.MinBindingSize,
                    Type = (BufferBindingType)entry.Buffer.Type
                };
            }

            if (entry.Texture != null)
            {
                newEntry.Texture = new TextureBindingLayout
                {
                    Multisampled = entry.Texture.Value.Multisampled,
                    SampleType = (TextureSampleType)entry.Texture.Value.SampleType,
                    ViewDimension = (TextureViewDimension)entry.Texture.Value.ViewDimension
                };
            }

            if (entry.Sampler != null)
            {
                newEntry.Sampler = new SamplerBindingLayout
                {
                    Type = (SamplerBindingType)entry.Sampler.Value.Type
                };
            }

            entries[i] = newEntry;
        }

        var bindGroupLayoutDescriptor = new Silk.NET.WebGPU.BindGroupLayoutDescriptor
        {
            Entries = entries,
            EntryCount = (nuint)descriptor.Entries.Length
        };

        if (descriptor.Label != null)
        {
            bindGroupLayoutDescriptor.Label = (byte*)SilkMarshal.StringToPtr(descriptor.Label);
        }

        return new GPUBindGroupLayout
        {
            BindGroupLayout = GPU.API.DeviceCreateBindGroupLayout(Device, in bindGroupLayoutDescriptor),
        };
    }

    public IGPUShaderModule CreateShaderModule(ShaderModuleDescriptor descriptor)
    {
        var wgslDescriptor = new ShaderModuleWGSLDescriptor
        {
            Code = (byte*)SilkMarshal.StringToPtr(descriptor.Code),
            Chain = new ChainedStruct
            {
                SType = SType.ShaderModuleWgslDescriptor
            }
        };

        var shaderModuleDescriptor = new Silk.NET.WebGPU.ShaderModuleDescriptor
        {
            NextInChain = (ChainedStruct*) (&wgslDescriptor)
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

        var viewFormat = TextureFormat.Rgba8Unorm;

        var textureDescriptor = new Silk.NET.WebGPU.TextureDescriptor
        {
            Format = (TextureFormat)descriptor.Format,
            Size = size,
            Usage = (TextureUsage)descriptor.Usage,
            SampleCount = 1,
            Dimension = TextureDimension.Dimension2D,
            MipLevelCount = 1,
            ViewFormats = &viewFormat,
            ViewFormatCount = 1,
            Label = (byte*)SilkMarshal.StringToPtr("MyTextureDescriptor")
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

        Console.WriteLine(new IntPtr(bindGroupLayouts[0]));
        Console.WriteLine(new IntPtr(bindGroupLayouts[1]));

        var pipelineLayoutDescriptor = new Silk.NET.WebGPU.PipelineLayoutDescriptor
        {
            BindGroupLayouts = bindGroupLayouts,
            BindGroupLayoutCount = (nuint)descriptor.BindGroupLayouts.Length
        };

        Console.WriteLine(pipelineLayoutDescriptor.BindGroupLayoutCount);

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
            MagFilter = (FilterMode)(descriptor.MagFilter ?? Client.WebGPU.FilterMode.Linear),
            MaxAnisotropy = 1
        };

        return new GPUSampler
        {
            Sampler = GPU.API.DeviceCreateSampler(Device, in samplerDescriptor)
        };
    }

    public void PushErrorScope()
    {
        GPU.API.DevicePushErrorScope(Device, ErrorFilter.Validation);
        GPU.API.DevicePushErrorScope(Device, ErrorFilter.OutOfMemory);
    }

    public void PopErrorScope()
    {
        GPU.API.DevicePopErrorScope(Device, new PfnErrorCallback((errorType, message, userData) =>
        {
            Console.WriteLine($"{errorType}: {SilkMarshal.PtrToString(new IntPtr(message))}");
        }), null);
    }
}