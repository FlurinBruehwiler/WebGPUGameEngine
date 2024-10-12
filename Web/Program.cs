using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Client;
using Client.WebGPU;
using Shared;
using Web.WebGPU;

namespace Web;

public static class Program
{
    public static async Task Main()
    {
        try
        {
            Game.GameInfo = await InitializeGame();
            Game.GameInfo.NullTexture = await Game.GameInfo.ResourceManager.LoadTexture("NullTexture.png");
            Game.GameInfo.Camera = new Camera
            {
                Transform = new Transform
                {
                    Position = new Vector3(10, 0, 0),
                    Scale = Vector3.One,
                    Rotation = new Vector3(0, MathF.PI / 2, 0),
                }
            };

            var cubeModel = await Game.GameInfo.ResourceManager.LoadModel("crate.obj");
            Renderer.UploadModel(cubeModel);
            cubeModel.SolidColor = Color.Beige;

            // cubeModel.Texture = await ResourceManager.LoadTexture("crate-texture.jpg");

            for (int i = 0; i < 10; i++)
            {
                Game.GameInfo.Entities.Add(new Entity
                {
                    Transform = new Transform
                    {
                        Scale = Vector3.One,
                        Position = Game.RandomVector(-25, 25) with { Y = 1 }
                    },
                    Model = cubeModel,
                });
            }

            var planeModel = await Game.GameInfo.ResourceManager.LoadModel("plane.obj");
            Renderer.UploadModel(planeModel);
            planeModel.Texture = await Game.GameInfo.ResourceManager.LoadTexture("grass.png");
            planeModel.SolidColor = Color.Green;

            Game.GameInfo.Entities.Add(new Entity
            {
                Transform = Transform.Default(30) with
                {
                    Position = new Vector3(-10, 0, -10)
                },
                Model = planeModel
            });

            Game.GameInfo.Server?.ListenForMessages();

            Game.GameInfo.Server?.SendMessage(
                new CreateMessage //should probably happen automatically, when an entity is added to the game..
                {
                    ModelId = "player",
                    Transform = new NetworkTransform(), //will be updated on the first frame
                    EntityId = Game.PlayerGuid
                });

            JsWindow.RequestAnimationFrame(FrameCatch);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void FrameCatch()
    {
        try
        {
            Game.Frame();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        JsWindow.RequestAnimationFrame(FrameCatch);
    }

    private static void HandleKeyEvent(string code, bool isDown)
    {
        KeyboardKeys key = KeyboardKeys.Unknown;

        switch (code)
        {
            case "KeyA":
                key = KeyboardKeys.A;
                break;
            case "KeyB":
                key = KeyboardKeys.B;
                break;
            case "KeyC":
                key = KeyboardKeys.C;
                break;
            case "KeyD":
                key = KeyboardKeys.D;
                break;
            case "KeyE":
                key = KeyboardKeys.E;
                break;
            case "KeyF":
                key = KeyboardKeys.F;
                break;
            case "KeyG":
                key = KeyboardKeys.G;
                break;
            case "KeyH":
                key = KeyboardKeys.H;
                break;
            case "KeyI":
                key = KeyboardKeys.I;
                break;
            case "KeyJ":
                key = KeyboardKeys.J;
                break;
            case "KeyK":
                key = KeyboardKeys.K;
                break;
            case "KeyL":
                key = KeyboardKeys.L;
                break;
            case "KeyM":
                key = KeyboardKeys.M;
                break;
            case "KeyN":
                key = KeyboardKeys.N;
                break;
            case "KeyO":
                key = KeyboardKeys.O;
                break;
            case "KeyP":
                key = KeyboardKeys.P;
                break;
            case "KeyQ":
                key = KeyboardKeys.Q;
                break;
            case "KeyR":
                key = KeyboardKeys.R;
                break;
            case "KeyS":
                key = KeyboardKeys.S;
                break;
            case "KeyT":
                key = KeyboardKeys.T;
                break;
            case "KeyU":
                key = KeyboardKeys.U;
                break;
            case "KeyV":
                key = KeyboardKeys.V;
                break;
            case "KeyW":
                key = KeyboardKeys.W;
                break;
            case "KeyX":
                key = KeyboardKeys.X;
                break;
            case "KeyY":
                key = KeyboardKeys.Y;
                break;
            case "KeyZ":
                key = KeyboardKeys.Z;
                break;
            case "Space":
                key = KeyboardKeys.Space;
                break;
            case "Enter":
                key = KeyboardKeys.Enter;
                break;
            case "ArrowUp":
                key = KeyboardKeys.Up;
                break;
            case "ArrowDown":
                key = KeyboardKeys.Down;
                break;
            case "ArrowLeft":
                key = KeyboardKeys.Left;
                break;
            case "ArrowRight":
                key = KeyboardKeys.Right;
                break;
            case "Escape":
                key = KeyboardKeys.Escape;
                break;
            case "Backspace":
                key = KeyboardKeys.Backspace;
                break;
            case "Tab":
                key = KeyboardKeys.Tab;
                break;
            case "Delete":
                key = KeyboardKeys.Delete;
                break;
            case "ShiftLeft":
            case "ShiftRight":
                key = KeyboardKeys.ShiftLeft; // You can handle left/right shift keys separately if needed
                break;
            case "ControlLeft":
            case "ControlRight":
                key = KeyboardKeys.ControlLeft; // You can handle left/right control keys separately if needed
                break;
            case "AltLeft":
            case "AltRight":
                key = KeyboardKeys.AltLeft; // You can handle left/right alt keys separately if needed
                break;
            default:
                key = KeyboardKeys.Unknown;
                break;
        }

        Game.GameInfo.Input.InformKeyChanged(key, isDown);
    }

    private static async Task<GameInfo> InitializeGame()
    {
        JsDocument.AddEventListener<KeyboardEvent>("keydown", e => HandleKeyEvent(e.Code, true));
        JsDocument.AddEventListener<KeyboardEvent>("keyup", e => HandleKeyEvent(e.Code, false));

        var canvas = JsDocument.QuerySelector<JsCanvas>("#gpuCanvas");

        canvas.AddEventListener<MouseEvent>("mousemove", e =>
        {
            if (!Game.StartedUp())
                return;

            Game.GameInfo.Input.MouseChangeX += e.MovementX;
            Game.GameInfo.Input.MouseChangeY += e.MovementY;
        });

        var adapter = await GPU.RequestAdapter();
        var device = await adapter.RequestDevice();

        var context = canvas.GetContext();


        var presentationFormat = GPU.GetPreferredCanvasFormat();
        context.Configure(new ContextConfig
        {
            Device = device,
            Format = presentationFormat,
            AlphaMode = "premultiplied"
        });

        return await Game.InitializeGame(new WebImplementation(context), device, presentationFormat);
    }
}