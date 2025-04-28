using System.Numerics;

namespace Ray_Tracing;

public struct Transform()
{
    public readonly Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Quaternion.Normalize(Rotation));
    public readonly Vector3 Right => Vector3.Transform(Vector3.UnitX, Quaternion.Normalize(Rotation));
    public readonly Vector3 Up => Vector3.Transform(Vector3.UnitY, Quaternion.Normalize(Rotation));

    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;
}
