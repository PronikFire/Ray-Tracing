namespace Ray_Tracing;

public class Transform
{
    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Position = Vector3.Zero;
    public Matrix4x4 Rotation = Matrix4x4.Identity;

    /// <summary>
    /// Rotate an object by a certain angle along a given axis.
    /// </summary>
    /// <param name="axis">The axis along which to rotate.</param>
    /// <param name="angle">The angle to turn. In degrees.</param>
    public void Rotate(Vector3 axis, float angle) => Rotation *= Matrix4x4.CreateFromAxisAngle(axis, angle * MathF.PI / 180);
    /// <summary>
    /// Rotate an object by Euler angles
    /// </summary>
    /// <param name="euler">Euler angles</param>
    public void Rotate(Vector3 euler) => Rotation *= Matrix4x4.CreateFromYawPitchRoll(euler.Y * MathF.PI / 180, euler.X * MathF.PI / 180, euler.Z * MathF.PI / 180);
}
