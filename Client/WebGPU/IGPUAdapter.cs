namespace GameEngine.WebGPU;

public interface IGPUAdapter
{
    Task<IGPUDevice> RequestDevice();
}