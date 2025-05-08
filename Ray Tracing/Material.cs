using Ray_Tracing.Objects;
using System;
using System.Drawing;
using System.Numerics;
using static System.Formats.Asn1.AsnWriter;

namespace Ray_Tracing;

public class Material()
{
    public Color color = Color.White;

    private static readonly Color Black = Color.Black;

    protected virtual Color OnCalculate(Vector3 point, Vector3 ray, Vector3 normal, MeshRender meshRender, Scene scene, uint raysLeft)
    {
        float r = 0, g = 0, b = 0;

        foreach (Object obj in scene.Objects)
        {
            if (obj is not Light light)
                continue;

            if (light.Intensity == 0)
                continue;

            Vector3 localLightPos = light.transform.position - point;
            float squaredLightDistance = localLightPos.LengthSquared();

            if (squaredLightDistance > light.Range * light.Range)
                continue;

            if (Vector3.Dot(normal, localLightPos) <= 0)
                continue;

            float brightness = light.Intensity / MathF.Sqrt(squaredLightDistance);

            if (scene.Raycast(point, Vector3.Normalize(localLightPos), out _, [meshRender]))
                continue;

            r = Math.Min(r + (int)(light.color.R * brightness), 255);
            g = Math.Min(g + (int)(light.color.G * brightness), 255);
            b = Math.Min(b + (int)(light.color.B * brightness), 255);
        }

        return Color.FromArgb((int)(color.R * r / 255f), (int)(color.G * g / 255f), (int)(color.B * b / 255f));
    }

    public Color CalculateColor(Vector3 point, Vector3 ray, Vector3 normal, MeshRender meshRender, Scene scene, uint raysLeft)
    {
        if (raysLeft == 0)
            return Black;

        return OnCalculate(point, ray, normal, meshRender, scene, raysLeft - 1);
    }
}
