using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace Ray_Tracing.Objects;

public class Camera(float fov, Size resolution, Scene scene) : Object
{
    public float Fov
    {
        get => fov;
        set => fov = MathF.Max(value, 0);
    }
    public Size Resolution
    {
        get => resolution;
        set
        {
            resolution = value;
        }
    }

    public Scene scene = scene;

    public int MaxReflections
    {
        get => maxReflections;
        set => maxReflections = Math.Max(value, 0);
    }
    public Color Background = Color.FromArgb(120, 120, 120);

    private float fov = MathF.Max(fov, 0);
    private Size resolution = resolution;
    private int maxReflections = 2;

    public Color GetPixel(int x, int y)
    {
        if (x >= resolution.Width || y >= resolution.Height)
            throw new ArgumentException();

        Quaternion rayRotation = Quaternion.CreateFromAxisAngle(transform.Up, (x / (float)resolution.Width - 0.5f) * Fov * (MathF.PI / 180));
        rayRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(transform.Right, (y / (float)resolution.Height - 0.5f) * Fov * (MathF.PI / 180)), rayRotation);
        
        if (!scene.Raycast(transform.Position, Vector3.Transform(transform.Forward, rayRotation), out var result))
            return Background;


        Color lightSum = Color.Black;
        foreach (Object @object in scene.Objects)
        {
            if (@object is not Light light)
                continue;

            if (light.Intensity == 0)
                continue;

            Vector3 localLightPos = light.transform.Position - result.point;

            float brightnessDot = Vector3.Dot(Vector3.Normalize(result.normal), Vector3.Normalize(localLightPos));
            if (brightnessDot <= 0)
                continue;

            if (localLightPos.Length() == 0)
                continue;

            float brightness = light.Intensity / localLightPos.Length();

            if (scene.Raycast(result.point, Vector3.Normalize(localLightPos), out var rayToLightResult, [result.meshRender]))
                brightness = 0;

            lightSum = Color.FromArgb(Math.Min(lightSum.R + (int)(light.Color.R * brightness), 255), Math.Min(lightSum.G + (int)(light.Color.G * brightness), 255), Math.Min(lightSum.B + (int)(light.Color.B * brightness), 255));
        }

        if (lightSum == Color.Black)
            return Color.Black;

        float dCamera = (Vector3.Dot(Vector3.Normalize(result.normal), Vector3.Normalize(transform.Position - result.meshRender.transform.Position)) + 1) / 2;

        return Color.FromArgb((int)Math.Min((result.meshRender.Material.Color.R + lightSum.R) * dCamera, 255), (int)Math.Min((result.meshRender.Material.Color.G + lightSum.G) * dCamera, 255), (int)Math.Min((result.meshRender.Material.Color.B + lightSum.B) * dCamera, 255));
    }
}
