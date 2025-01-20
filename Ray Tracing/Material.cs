namespace Ray_Tracing;

public class Material()
{
    public Material(Material material) : this()
    {
        Color = material.Color;
        glowIntensity = material.GlowIntensity;
    }

    public Color Color = Color.White;
    public float GlowIntensity
    {
        get => glowIntensity;
        set => glowIntensity = MathF.Max(value, 0);
    }
    public float Specularity
    {
        get => specularity;
        set => specularity = Math.Clamp(value, 0, 1);
    }

    private float glowIntensity = 0;
    private float specularity = 0;
}
