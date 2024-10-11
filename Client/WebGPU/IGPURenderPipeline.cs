namespace GameEngine.WebGPU;

public interface IGPURenderPipeline
{
    IGPUBindGroupLayout GetBindGroupLayout(int index);
}