using System;
using System.Drawing;

namespace Ray_Tracing.Objects;

public class Light : Object
{
    public float Intensity
    {
        get => intensity;
        set => intensity = MathF.Max(value, 0);
    }
    public float Range
    {
        get => range;
        set => range = MathF.Max(range, 0);
    }

    public Color color = Color.White;

    private float intensity = 1;
    private float range = 10;
}
