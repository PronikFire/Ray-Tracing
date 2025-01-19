namespace Ray_Tracing;

public struct IntersectionResult(Vector3 point, Vector3 normal, Object obj, Material material)
{ 
    public Vector3 Point = point;
    public Vector3 Normal = normal;
    public Material Material = material;
    public Object Object = obj;
}
