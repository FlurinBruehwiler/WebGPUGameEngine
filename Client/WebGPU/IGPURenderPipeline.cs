namespace Client.WebGPU;

public interface IGPURenderPipeline
{
    IGPUBindGroupLayout GetBindGroupLayout(int index);
}