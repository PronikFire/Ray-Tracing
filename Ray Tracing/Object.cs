namespace Ray_Tracing;

public abstract class Object : IDisposable
{
    public Vector3 Position = Vector3.Zero;
    public Matrix4x4 Rotation = Matrix4x4.Identity;

    public void Dispose() => GC.SuppressFinalize(this);
}
