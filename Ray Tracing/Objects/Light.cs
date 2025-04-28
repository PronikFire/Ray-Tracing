using System;
using System.Drawing;
using System.Numerics;

namespace Ray_Tracing.Objects;

public class Light : Object
{
    public float Intensity
    {
        get => intensity;
        set => intensity = MathF.Max(value, 0);
    }

    public Color Color = Color.White;

    private float intensity = 1;
}
