namespace Ray_Tracing;

public struct Ray(Vector3 origin, Vector3 direction)
{
    public Vector3 Origin = origin;
    public Vector3 Direction = direction;

    public Vector3 PointByTime(float t) => Origin + Vector3.Normalize(Direction) * t;
}
