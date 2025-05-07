using System;
using System.Drawing;
using System.Numerics;

namespace Ray_Tracing.Objects;

public class Camera : Object
{
    public float Fov
    {
        get => fov;
        set => fov = MathF.Max(value, 0);
    }

    public uint maxReflections = 2;
    public Color background = Color.FromArgb(120, 120, 120);
    public Size resolution = new(1920, 1080);

    private float fov = 60;

    public Color GetPixel(int x, int y)
    {
        if (x >= resolution.Width || y >= resolution.Height)
            throw new ArgumentException();

        if (Scene is null)
            throw new Exception("Scene is null");

        //fucking fish eye
        float xPivot = x / (float)resolution.Width - 0.5f;
        float yPivot = y / (float)resolution.Height - 0.5f;
        Quaternion rayRotation = Quaternion.CreateFromAxisAngle(transform.Up, xPivot * Fov / 2 * MathF.PI / 180);
        rayRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(transform.Right, yPivot * Fov / 2 * MathF.PI / 180), rayRotation);
        
        if (!Scene.Raycast(transform.position, Vector3.Transform(transform.Forward, rayRotation), out var result))
            return background;

        return result.meshRender.material.CalculateColor(result.point, result.point - transform.position, result.normal, result.meshRender, Scene, maxReflections);
    }
}
