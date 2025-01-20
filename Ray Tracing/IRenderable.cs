namespace Ray_Tracing;

public interface IRenderable
{
    public Material Material { get; set; }
    public bool Intersection(Ray ray, out IntersectionResult result);
    public bool Intersection(Ray ray) => Intersection(ray, out IntersectionResult _);
    public async Task<bool> IntersectionAsync(Ray ray) => Intersection(ray);
}
