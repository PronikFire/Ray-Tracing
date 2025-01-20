using Ray_Tracing.Objects;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

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

    /// <summary>
    /// Add an object to the scene
    /// </summary>
    /// <param name="obj">The object that should be added</param>
    public void AddObject(Object obj)
    {
        if (objects.Contains(obj))
            throw new Exception("This object has already been added to the scene.");

        objects.Add(obj);
        switch (obj)
        {
            case Light light:
                lights.Add(light);
                break;
        }
    }
    /// <summary>
    /// Remove object from scene.
    /// </summary>
    /// <param name="obj">The object that needs to be removed.</param>
    public void RemoveObject(Object obj)
    {
        if (!lights.Contains(obj))
            throw new Exception("There is no such object on the scene.");

        objects.Remove(obj);
        switch (obj)
        {
            case Light light:
                lights.Remove(light);
                break;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void RemoveObject(int index)
    {
        if (index < 0 || index >= objects.Count)
            throw new ArgumentOutOfRangeException("index");

        RemoveObject(objects[index]);
    }

}
