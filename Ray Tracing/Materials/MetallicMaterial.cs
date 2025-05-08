using Ray_Tracing.Objects;
using System;
using System.Drawing;
using System.Numerics;

namespace Ray_Tracing.Materials;

public class MetallicMaterial : Material
{
    public float Roughness
    {
        get => roughness;
        set => roughness = Math.Clamp(value, 0, 1);
    }

    public float Reflectivity
    {
        get => reflectivity;
        set => reflectivity = Math.Clamp(value, 0, 1);
    }

    private float roughness = 0.1f;
    private float reflectivity = 0.8f;

    protected override Color OnCalculate(Vector3 point, Vector3 ray, Vector3 normal, MeshRender meshRender, Scene scene, uint raysLeft)
    {
        Color baseColor = base.OnCalculate(point, ray, normal, meshRender, scene, raysLeft);

        Vector3 reflectedDirection = Vector3.Reflect(Vector3.Normalize(ray), normal);

        if (roughness > 0)
        {
            Quaternion rayRotation = Quaternion.CreateFromYawPitchRoll((float)((Random.Shared.NextDouble() - 0.5) * 2 * roughness * 90 * MathF.PI / 180),
                (float)((Random.Shared.NextDouble() - 0.5) * 2 * roughness * 90 * MathF.PI / 180), 0);
            reflectedDirection = Vector3.Transform(reflectedDirection, rayRotation);
            if (Vector3.Dot(reflectedDirection, normal) <= 0)
                return baseColor;
        }

        if (!scene.Raycast(point, reflectedDirection, out var result, [meshRender]))
            return baseColor;

        Color reflectedColor = result.meshRender.material.CalculateColor(
                result.point,
                reflectedDirection,
                result.normal,
                result.meshRender,
                scene,
                raysLeft);

        return Color.FromArgb(
            (int)(baseColor.R * (1 - reflectivity) + reflectedColor.R * reflectivity),
            (int)(baseColor.G * (1 - reflectivity) + reflectedColor.G * reflectivity),
            (int)(baseColor.B * (1 - reflectivity) + reflectedColor.B * reflectivity));
    }
}
