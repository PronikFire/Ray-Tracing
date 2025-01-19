namespace Ray_Tracing.Objects;

public class Sphere(float radius) : Object, IRenderable
{
    public Material Material { get; set; } = new ();

    public float Radius = radius;

    public bool Intersection(Ray ray, out IntersectionResult result)
    {
        result = default;

        Vector3 v = ray.Origin - Position;
        float b = 2 * Vector3.Dot(Vector3.Normalize(ray.Direction), v);
        float c = Vector3.Dot(v, v)- Radius * Radius;
        float d = b * b - 4 * c;

        if (d < 0)
            return false;

        d = MathF.Sqrt(d);

        float t0 = (-b + d) / 2;
        float t1 = (-b - d) / 2;

        if (t0 < 0 && t1 < 0)
            return false;

        result.Point = ray.Origin + Vector3.Normalize(ray.Direction) * ((t0 < t1 && t0 >= 0) ? t0 : t1);
        result.Normal = Vector3.Normalize(result.Point - Position);
        result.Material = Material;
        result.Object = this;
        return true;
    }
}
