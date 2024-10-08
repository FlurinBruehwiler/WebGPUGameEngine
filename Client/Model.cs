using System.Drawing;
using System.Numerics;
using GameEngine.WebGPU;

namespace GameEngine;

public struct Vertex
{
    public required Vector3 Position;
    public required Vector2 Texcoord;
}

public struct Plane
{
    public Vector3 Normal; //slope of the plane
    public float Scalar; //offset from origin along the normal

    public Vector3 GetPointOnPlane()
    {
        return Normal * Scalar;
    }

    public bool IsOnPlane(Vector3 position)
    {
        return Normal.X * position.X +
            Normal.Y * position.Y +
            Normal.Z * position.Z
            + Scalar == 0;
    }
}

public struct CollisionBox
{

}

public class Model : IDisposable
{
    public required Vertex[] Vertices;
    public CollisionBox CollisionBox;
    public GPUBuffer? GpuBuffer;
    public Texture? Texture;
    public Color SolidColor;
    public GPUBindGroup? TextureBindGroup;

    public void Dispose()
    {
        GpuBuffer?.Destory();
    }
}