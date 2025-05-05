using System.Numerics;

namespace Ray_Tracing;

public struct Transform()
{
    public readonly Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Quaternion.Normalize(rotation));
    public readonly Vector3 Right => Vector3.Transform(Vector3.UnitX, Quaternion.Normalize(rotation));
    public readonly Vector3 Up => Vector3.Transform(Vector3.UnitY, Quaternion.Normalize(rotation));

    public Vector3 position = Vector3.Zero;
    public Quaternion rotation = Quaternion.Identity;
    public Vector3 scale = Vector3.One;
}
