using System.Numerics;
using GameEngine.WebGPU;

namespace GameEngine;

public struct Transform
{
    public Vector3 Position;
    public required Vector3 Scale;
    public Vector3 Rotation;

    public static Transform Default(float scale = 1)
    {
        return new Transform
        {
            Scale = new Vector3(scale)
        };
    }

    public static Transform Create(Vector3 position, Vector3 rotation, float scale = 1)
    {
        return new Transform
        {
            Scale = new Vector3(scale),
            Position = position,
            Rotation = rotation
        };
    }

    public Matrix4x4 ToMatrix()
    {
        var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);

        var cameraRotationMatrix = Matrix4x4.CreateRotationY(Rotation.Y) * Matrix4x4.CreateRotationZ(Rotation.Z) *
                                   Matrix4x4.CreateRotationX(Rotation.X);

        var translationMatrix = Matrix4x4.CreateTranslation(Position);

        return scaleMatrix * cameraRotationMatrix * translationMatrix;
    }
}

public class Entity
{
    public required Model Model;
    public required Transform Transform;
}

public class GameInfo
{
    public List<Vector3> ImmediateVertices = [];
    // public List<Model> Models = [];
    public List<Entity> Entities = [];
    public required GPURenderPipeline RenderPipeline;
    public required GPUDevice Device;
    public required GPUCanvasContext Context;
    public int ScreenWidth;
    public int ScreenHeight;
    public required JsCanvas JsCanvas;
    public Camera Camera;
    public Input Input = new();

    public void UpdateScreenDimensions()
    {
        ScreenWidth = JsCanvas.JsObject.GetPropertyAsInt32("width");
        ScreenHeight = JsCanvas.JsObject.GetPropertyAsInt32("height");
    }
}

public class Input
{
    private HashSet<string> pressedKeys = [];

    public float MouseChangeX;
    public float MouseChangeY;

    public void InformKeyChanged(string code, bool down)
    {
        if (down)
        {
            pressedKeys.Add(code);
        }
        else
        {
            pressedKeys.Remove(code);
        }
    }

    public bool IsKeyDown(string code)
    {
        return pressedKeys.Contains(code);
    }

    public void NextFrame()
    {
        MouseChangeX = 0;
        MouseChangeY = 0;
    }
}
