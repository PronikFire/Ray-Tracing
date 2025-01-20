using SFML.System;
using System.Threading.Tasks;

namespace Ray_Tracing.Objects;

public class Camera(float fov, Vector2u size, Scene scene) : Object
{
    public float Fov
    {
        get => fov;
        set => fov = MathF.Max(value, 0);
    }
    public Vector2u Size
    {
        get => size;
        set
        {
            size = value;
            pixels = new Color[value.X, value.Y];
        }
    }
    public Scene Scene = scene;
    public int MaxReflections
    {
        get => maxReflections;
        set => maxReflections = Math.Min(value, 0);
    }

    private float fov = MathF.Max(fov, 0);
    private Vector2u size = size;
    private int maxReflections = 2;

    private Color[,] pixels = new Color[size.X, size.Y];


    public async Task<Color[,]> GetImageAsync()
    {
        for (uint x = 0; x < size.X; x++)
        {
            Task[] rays = new Task[size.Y];
            for (uint y = 0; y < size.Y; y++)
            {
                rays[y] = Raycast(x, y);
            }
            await Task.WhenAll(rays);
        }

        return pixels;
    }

    public Color[,] GetImage()
    {
        GetImageAsync().Wait();
        return pixels;
    }

    private async Task Raycast(uint x, uint y)
    {
        Matrix4x4 rayRotation = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, (x / (float)size.X - 0.5f) * Fov * (MathF.PI / 180)) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, (y / (float)size.Y - 0.5f) * Fov * (MathF.PI / 180));
        Ray ray = new(Transform.Position, Vector3.Transform(Vector3.UnitZ, rayRotation * Transform.Rotation));

        IntersectionResult result;

        {
            IntersectionResult? currentResult = null;

            foreach (Object obj in Scene.Objects)
            {
                if (obj is not IRenderable renderable)
                    continue;

                if (!renderable.Intersection(ray, out IntersectionResult newResult))
                    continue;

                if (currentResult != null && Vector3.Distance(Transform.Position, newResult.Point) > Vector3.Distance(Transform.Position, currentResult.Value.Point))
                    continue;

                currentResult = newResult;
            }

            if (currentResult == null)
            {
                pixels[x, y] = Scene.Background;
                return;
            }
            result = currentResult.Value;
        }

        Color lightSum = Scene.Background;
        foreach (Light light in Scene.Lights)
        {
            if (light.Intensity == 0)
                continue;

            Vector3 localLightPos = light.Transform.Position - result.Point;

            float brightnessDot = Vector3.Dot(Vector3.Normalize(result.Normal), Vector3.Normalize(localLightPos));
            if (brightnessDot <= 0)
                continue;

            if (localLightPos.Length() == 0)
                continue;

            byte brightness = (byte)(MathF.Min(light.Intensity / localLightPos.Length(), 1) * 255 * brightnessDot);
            foreach (var obj in Scene.Objects)
            {
                if (obj == result.Object)
                    continue;

                if (obj is not IRenderable renderable)
                    continue;

                if (!renderable.Intersection(new(result.Point, Vector3.Normalize(localLightPos))))
                    continue;

                brightness = 0;
                break;
            }
            lightSum += light.Color * new Color(brightness, brightness, brightness);
        }
        byte dCamera = (byte)(Vector3.Dot(Vector3.Normalize(result.Normal), Vector3.Normalize(Transform.Position - result.Object.Transform.Position)) * 127 + 127);

        pixels[x, y] = result.Material.Color * lightSum * new Color(dCamera, dCamera, dCamera);
    }
}
