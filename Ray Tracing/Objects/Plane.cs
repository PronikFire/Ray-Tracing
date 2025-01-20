using System.Numerics;
using System;

namespace Ray_Tracing.Objects;

public class Plane() : Object, IRenderable
{
    public float D
    {
        get
        {
            float sqrMag = Vector3.Dot(Transform.Up, Transform.Up);
            if (sqrMag < float.Epsilon)
                return 0;

            var dot = Vector3.Dot(Transform.Position, Transform.Up);
            return (Transform.Up * (dot / sqrMag)).Length();
        }
        set => Transform.Position = Transform.Up * value;
    }
    public Material Material { get; set; } = new();

    public bool Intersection(Ray ray, out IntersectionResult result)
    {
        result = default;

        float product1 = Vector3.Dot(ray.Origin, Transform.Up);
        float product2 = Vector3.Dot(Transform.Up, ray.Direction);
        float t = (-D - product1) / product2;

        if (t < 0)
            return false;

        result.Point = ray.PointByTime(t);
        result.Normal = Transform.Up;
        result.Material = Material;
        //result.Material = new Material(Material) { Color = new Color((uint)((MathF.Sin(result.Point.X) * MathF.Sin(result.Point.Y)) * uint.MaxValue)) };
        result.Object = this;

        return true;
    }
}
