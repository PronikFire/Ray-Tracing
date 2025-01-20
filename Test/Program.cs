using Ray_Tracing.Objects;
using SFML.Graphics;
using System.Numerics;
using Ray_Tracing;
using System.Threading.Tasks;
using System;
using SFML.System;
using SFML.Window;
using System.Diagnostics;

namespace Test;

public class Program
{
    private static readonly RenderWindow window = new(new(600, 600), "Ray Tracing");
    private static readonly Scene scene = new();
    private static readonly Camera camera = new(60, window.Size, scene);
    private static Image canvas = new (window.Size.X, window.Size.Y, Color.Black);
    private static readonly Matrix4x4 rotationDelta = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, 1f * (MathF.PI / 180));
    private static readonly Light light = new();
    private const float cameraSensitivity = 0.25f;
    private const float cameraSpeed = 1f;

    public static void Main()
    { 
        Texture texture = new (canvas) { Repeated = false, Smooth = false };
        Sprite sprite = new (texture);
        window.Closed += (object? o, EventArgs a) => window.Close();


        camera.Transform.Position = new Vector3(0, 50, -100);

        Sphere sphere1 = new(25);
        sphere1.Transform.Position = Vector3.UnitY * sphere1.Radius;
        sphere1.Material.Color = Color.Yellow;
        scene.AddObject(sphere1);

        Sphere sphere2 = new(7.5f);
        sphere2.Material.Color = Color.Green;
        sphere2.Transform.Position = new Vector3(50, sphere1.Transform.Position.Y, 0);
        scene.AddObject(sphere2);

        light.Transform.Position = new Vector3(75, sphere1.Transform.Position.Y + 50, 0);
        light.Intensity = 50;
        scene.AddObject(light);

        Ray_Tracing.Objects.Plane plane = new();
        scene.AddObject(plane);


        window.SetMouseCursorVisible(false);
        window.MouseMoved += OnMouseMove;

        Task updateTask = Update();

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear();

            updateTask.Wait();
            texture.Update(canvas);
            window.Draw(sprite);

            updateTask = Update();

            window.Display();
        }
    }

    private static void OnMouseMove(object? o, MouseMoveEventArgs a)
    {
        if (!window.HasFocus())
            return;

        camera.Transform.Rotate(Vector3.UnitY, a.X - window.Size.X / 2);
        camera.Transform.Rotate(camera.Transform.Right, a.Y - window.Size.Y / 2);
        Mouse.SetPosition((Vector2i)window.Size / 2, window);
    }

    private static async Task Update()
    {
        canvas.Dispose();
        canvas = new Image(await camera.GetImageAsync());

        light.Transform.ModifyPositionByMatrix(rotationDelta);

        Vector3 dPos = Vector3.Zero;
        if  (Keyboard.IsKeyPressed(Keyboard.Key.W) != Keyboard.IsKeyPressed(Keyboard.Key.S))
            dPos += camera.Transform.Forward * (Keyboard.IsKeyPressed(Keyboard.Key.W) ? 1 : -1);
        if (Keyboard.IsKeyPressed(Keyboard.Key.D) != Keyboard.IsKeyPressed(Keyboard.Key.A))
            dPos += camera.Transform.Right * (Keyboard.IsKeyPressed(Keyboard.Key.D) ? 1 : -1);
        if (Keyboard.IsKeyPressed(Keyboard.Key.Space) != Keyboard.IsKeyPressed(Keyboard.Key.LShift))
            dPos += Vector3.UnitY * (Keyboard.IsKeyPressed(Keyboard.Key.Space) ? 1 : -1);
        dPos *= cameraSpeed;
        camera.Transform.Position += dPos;
        //camera.Rotation = camera.Rotation * rotationDelta;
        //camera.Position = Vector3.Transform(camera.Position, rotationDelta);
    }
}
