using Ray_Tracing.Objects;
using System.Collections.ObjectModel;

namespace Ray_Tracing;

public class Scene
{
    public Color Background = new(120, 120, 120);
    public ReadOnlyCollection<Object> Objects => objects.AsReadOnly();
    public ReadOnlyCollection<Light> Lights => lights.AsReadOnly();

    private List<Object> objects = [];
    private List<Light> lights = [];

    /* I'm not sure it works
     * 
    public Object CreateObject<T>() where T : Object
    {
        Object newObject = default(T);
        AddObject(newObject);
        return newObject;
    }
    */

    public void AddObject(Object obj)
    {
        objects.Add(obj);
        switch (obj)
        {
            case Light light:
                lights.Add(light);
                break;
        }
    }
}
