using Ray_Tracing.Objects;
using System;
using System.Drawing;
using System.Numerics;
using static System.Formats.Asn1.AsnWriter;

namespace Ray_Tracing;

public class Material()
{
    public Color color = Color.White;

    protected virtual Color OnCalculate(Vector3 point, Vector3 ray, Vector3 normal, MeshRender meshRender, Scene scene, uint raysLeft)
    {
        Color lightSum = Color.Black;
        foreach (Object obj in scene.Objects)
        {
            if (obj is not Light light)
                continue;

            if (light.Intensity == 0)
                continue;

            Vector3 localLightPos = light.transform.position - point;

            if (localLightPos.Length() > light.Range)
                continue;

            if (Vector3.Dot(normal, localLightPos) < 0)
                continue;

            float brightness = light.Intensity / localLightPos.Length();

            if (scene.Raycast(point, Vector3.Normalize(localLightPos), out var rayToLightResult, [meshRender]))
                continue;

            lightSum = Color.FromArgb(Math.Min(lightSum.R + (int)(light.color.R * brightness), 255), Math.Min(lightSum.G + (int)(light.color.G * brightness), 255), Math.Min(lightSum.B + (int)(light.color.B * brightness), 255));
        }

        return Color.FromArgb((int)(color.R * (lightSum.R / 255f)), (int)(color.G * (lightSum.G / 255f)), (int)(color.B * (lightSum.B / 255f)));
    }

    public Color CalculateColor(Vector3 point, Vector3 ray, Vector3 normal, MeshRender meshRender, Scene scene, uint raysLeft)
    {
        if (raysLeft == 0)
            return Color.Black;

        raysLeft--;
        return OnCalculate(point, ray, normal, meshRender, scene, raysLeft);
    }
}
