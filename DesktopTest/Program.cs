using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

unsafe
{
    var options = WindowOptions.Default;
    options.Size = new Vector2D<int>(800, 600);

    WebGPU wgpu = null!;
    Instance* instance;
    Surface* surface = null!;
    Adapter* adapter;
    Device* device = null!;
    Queue* queue = null!;

    var window = Window.Create(options);
    window.Load += onLoad;
    window.FramebufferResize += onResize;
    window.Render += onRender;
    window.Run();

    void onLoad()
    {
        wgpu = WebGPU.GetApi();
        instance = wgpu.CreateInstance(new InstanceDescriptor());
        surface = window.CreateWebGPUSurface(wgpu, instance);
        adapter = requestAdapter(wgpu, instance, new RequestAdapterOptions { CompatibleSurface = surface });
        device = requestDevice(wgpu, adapter, new DeviceDescriptor());
        queue = wgpu.DeviceGetQueue(device);

        onResize(window.FramebufferSize);
    }

    void onResize(Vector2D<int> size)
    {
        wgpu.SurfaceConfigure(surface, new SurfaceConfiguration
        {
            Device = device,
            Format = TextureFormat.Bgra8Unorm,
            Usage = TextureUsage.RenderAttachment,
            Width = (uint)size.X,
            Height = (uint)size.Y,
            PresentMode = PresentMode.Fifo
        });
    }

    void onRender(double delta)
    {
        SurfaceTexture surfaceTexture = default;
        wgpu.SurfaceGetCurrentTexture(surface, ref surfaceTexture);
        TextureView* surfaceView = wgpu.TextureCreateView(surfaceTexture.Texture, null);

        var encoder = wgpu.DeviceCreateCommandEncoder(device, new CommandEncoderDescriptor());

        RenderPassColorAttachment* colourAttachments = stackalloc RenderPassColorAttachment[1]
        {
            new RenderPassColorAttachment
            {
                View = surfaceView,
                LoadOp = LoadOp.Clear,
                StoreOp = StoreOp.Store,
                ClearValue = new Color(1.0, 0.0, 1.0, 1.0)
            }
        };

        var renderPassDescriptor = new RenderPassDescriptor
        {
            ColorAttachmentCount = 1,
            ColorAttachments = colourAttachments
        };

        var renderPass = wgpu.CommandEncoderBeginRenderPass(encoder, &renderPassDescriptor);

        wgpu.RenderPassEncoderEnd(renderPass);
        wgpu.RenderPassEncoderRelease(renderPass);
        wgpu.TextureViewRelease(surfaceView);

        CommandBuffer** commandBuffers = stackalloc CommandBuffer*[1];
        commandBuffers[0] = wgpu.CommandEncoderFinish(encoder, new CommandBufferDescriptor());
        wgpu.CommandEncoderRelease(encoder);

        wgpu.QueueSubmit(queue, 1, commandBuffers);
        wgpu.CommandBufferRelease(commandBuffers[0]);

        wgpu.SurfacePresent(surface);
    }

    static Adapter* requestAdapter(WebGPU wgpu, Instance* instance, RequestAdapterOptions options)
    {
        Adapter* result = null;

        using ManualResetEventSlim adapterRequestEvent = new ManualResetEventSlim();
        wgpu.InstanceRequestAdapter(instance, options, PfnRequestAdapterCallback.From((status, adapter, _, _) =>
        {
            if (status == RequestAdapterStatus.Success)
                result = adapter;

            // ReSharper disable once AccessToDisposedClosure
            adapterRequestEvent.Set();
        }), null);

        adapterRequestEvent.Wait();

        return result;
    }

    static Device* requestDevice(WebGPU wgpu, Adapter* adapter, DeviceDescriptor desc)
    {
        Device* result = null;

        using ManualResetEventSlim adapterRequestEvent = new ManualResetEventSlim();
        wgpu.AdapterRequestDevice(adapter, desc, PfnRequestDeviceCallback.From((status, device, _, _) =>
        {
            if (status == RequestDeviceStatus.Success)
                result = device;

            // ReSharper disable once AccessToDisposedClosure
            adapterRequestEvent.Set();
        }), null);

        adapterRequestEvent.Wait();

        return result;
    }
}