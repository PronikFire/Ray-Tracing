using System;
using System.Drawing;
using System.Numerics;

namespace Ray_Tracing.Objects;

public class Camera(float fov, Size resolution, Scene scene) : Object
{
    public float Fov
    {
        get => fov;
        set => fov = MathF.Max(value, 0);
    }

    public Scene scene = scene;
    public uint maxReflections = 2;
    public Color background = Color.FromArgb(120, 120, 120);
    public Size resolution = resolution;

    private float fov = MathF.Max(fov, 0);

    public Color GetPixel(int x, int y)
    {
        if (x >= resolution.Width || y >= resolution.Height)
            throw new ArgumentException();

        //fucking fish eye
        float xPivot = x / (float)resolution.Width - 0.5f;
        float yPivot = y / (float)resolution.Height - 0.5f;
        Quaternion rayRotation = Quaternion.CreateFromAxisAngle(transform.Up, xPivot * Fov / 2 * MathF.PI / 180);
        rayRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(transform.Right, yPivot * Fov / 2 * MathF.PI / 180), rayRotation);
        
        if (!scene.Raycast(transform.position, Vector3.Transform(transform.Forward, rayRotation), out var result))
            return background;


        Color lightSum = Color.Black;
        foreach (Object obj in scene.Objects)
        {
            if (obj is not Light light)
                continue;

            if (light.Intensity == 0)
                continue;

            Vector3 localLightPos = light.transform.position - result.point;

            if (localLightPos.Length() > light.Range)
                continue;

            if (Vector3.Dot(result.normal, localLightPos) < 0)
                continue;
            
            float brightness = light.Intensity / localLightPos.Length();

            if (scene.Raycast(result.point, Vector3.Normalize(localLightPos), out var rayToLightResult, [result.meshRender]))
                continue;

            lightSum = Color.FromArgb(Math.Min(lightSum.R + (int)(light.color.R * brightness), 255), Math.Min(lightSum.G + (int)(light.color.G * brightness), 255), Math.Min(lightSum.B + (int)(light.color.B * brightness), 255));
        }

        Color materialColor = result.meshRender.material.CalculateColor(result.point, result.point - transform.position, result.normal, maxReflections);

        return Color.FromArgb((int)(materialColor.R * (lightSum.R / 255f)), (int)(materialColor.G * (lightSum.G / 255f)), (int)(materialColor.B * (lightSum.B / 255f)));
    }
}
