namespace Ray_Tracing;

public class Material
{
    public Color Color = Color.White;
    public float GlowIntensity
    {
        get => glowIntensity;
        set => glowIntensity = MathF.Max(value, 0);
    }

    private float glowIntensity = 0;
}
