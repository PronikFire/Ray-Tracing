using System.Drawing;
using System.Numerics;

namespace Ray_Tracing;

public class Material()
{
    public Color color = Color.White;

    protected virtual Color OnCalculate(Vector3 point, Vector3 ray, Vector3 normal, uint raysLeft) => color;

    public Color CalculateColor(Vector3 point, Vector3 ray, Vector3 normal, uint raysLeft)
    {
        if (raysLeft == 0)
            return Color.Black;

        raysLeft--;
        return OnCalculate(point, ray, normal, raysLeft);
    }
}
