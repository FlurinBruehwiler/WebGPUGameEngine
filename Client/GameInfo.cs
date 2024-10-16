﻿using System.Numerics;
using Client.WebGPU;
using Shared;

namespace Client;

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

    public NetworkTransform ToNetwork()
    {
        return new NetworkTransform
        {
            Position = Position,
            Rotation = Rotation,
            Scale = Scale
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

    public static Transform FromNetwork(NetworkTransform transform)
    {
        return new Transform
        {
            Scale = transform.Scale,
            Position = transform.Position,
            Rotation = transform.Rotation,
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
    public Guid Id = Guid.NewGuid();
    public required Model Model;
    public required Transform Transform;
}

public class GameInfo
{
    public List<Vector3> ImmediateVertices = [];
    // public List<Model> Models = [];
    public List<Entity> Entities = [];
    public required IGPURenderPipeline RenderPipeline;
    public required IGPUDevice Device;

    public required IPlatformImplementation PlatformImplementation;
    // public required GPUCanvasContext Context;
    public int ScreenWidth;
    public int ScreenHeight;
    // public required IScreen Screen;
    public Camera Camera;
    public Input Input = new();
    public required Texture NullTexture;
    public Server? Server;
    public required ResourceManager ResourceManager;

    public void UpdateScreenDimensions()
    {
        // ScreenWidth = Screen.GetWidth();
        // ScreenHeight = Screen.GetHeight();
    }
}

public class Input
{
    private HashSet<KeyboardKeys> pressedKeys = [];

    public float MouseChangeX;
    public float MouseChangeY;

    public void InformKeyChanged(KeyboardKeys key, bool down)
    {
        if (down)
        {
            pressedKeys.Add(key);
        }
        else
        {
            pressedKeys.Remove(key);
        }
    }

    public bool IsKeyDown(KeyboardKeys key)
    {
        return pressedKeys.Contains(key);
    }

    public void NextFrame()
    {
        MouseChangeX = 0;
        MouseChangeY = 0;
    }
}
