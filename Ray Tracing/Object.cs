namespace Ray_Tracing;

public abstract class Object()
{
    public Scene? Scene
    {
        get => scene;
        internal set => scene = value;
    }

    public Transform transform = new();

    private Scene? scene;
}
