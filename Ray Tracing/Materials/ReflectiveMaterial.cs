using System.Drawing;
using System.Numerics;

namespace Ray_Tracing.Materials;

public class ReflectiveMaterial : Material
{
    public float reflectionDegree = 0.5f;

    protected override Color OnCalculate(Vector3 point, Vector3 ray, Vector3 normal, uint raysLeft)
    {
        if (raysLeft == 0)
            return Color.Black;

        

        return color;
    }
}
