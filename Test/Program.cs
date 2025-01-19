using Ray_Tracing.Objects;
using SFML.Graphics;
using System.Numerics;
using Ray_Tracing;
using System.Threading.Tasks;
using System;

namespace Test;

public class Program
{
    private static readonly RenderWindow window = new(new(600, 600), "Ray Tracing");
    private static readonly Scene scene = new();
    private static readonly Camera camera = new(90, window.Size, scene) { Position = new Vector3(0, 0, -100) };
    private static Image canvas = new (window.Size.X, window.Size.Y, Color.Black);
    private static readonly Matrix4x4 rotationDelta = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, 5f * (MathF.PI / 180));

    public static void Main(string[] args)
    { 
        Texture texture = new (canvas) { Repeated = false, Smooth = false };
        Sprite sprite = new (texture);
        window.Closed += (object? o, EventArgs e) => window.Close();

        Sphere sphere1 = new(25);
        sphere1.Material.Color = Color.Yellow;
        scene.AddObject(sphere1);

        Sphere sphere2 = new(7.5f);
        sphere2.Material.Color = Color.Green;
        sphere2.Position = Vector3.UnitX * 50;
        scene.AddObject(sphere2);

        Light light = new();
        light.Position = new Vector3(25, 75, 10);
        light.Intensity = 50;
        scene.AddObject(light);

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

    private static async Task Update()
    {
        canvas.Dispose();
        canvas = new Image(await camera.GetImageAsync());

        //light = Vector3.Transform(light, rotationDelta);
        camera.Rotation = camera.Rotation * rotationDelta;
        camera.Position = Vector3.Transform(camera.Position, rotationDelta);
    }
}
