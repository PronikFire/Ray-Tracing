namespace Ray_Tracing;

public interface IRenderable
{
    //public static async Task<bool>
    //
    //IntersectionAsync(Ray ray, IRenderable renderable) => renderable.Intersection(ray, out IntersectionResult _);
    //public static bool Intersection(Ray ray, IRenderable renderable) => renderable.Intersection(ray, out IntersectionResult _);

    public Material Material { get; set; }
    public bool Intersection(Ray ray, out IntersectionResult result);
    public bool Intersection(Ray ray) => Intersection(ray, out IntersectionResult _);
    public async Task<bool> IntersectionAsync(Ray ray) => Intersection(ray);
}
