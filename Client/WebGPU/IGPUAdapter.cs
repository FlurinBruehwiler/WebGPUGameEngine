namespace Client.WebGPU;

public interface IGPUAdapter
{
    Task<IGPUDevice> RequestDevice();
}