using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic reusable object pool. Reduces instantiation overhead and memory
/// fragmentation on mobile by recycling GameObjects instead of destroying them.
/// Attach a concrete subclass as a Singleton component in the scene.
/// </summary>
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> available = new Queue<T>();

    public int CountActive   { get; private set; }
    public int CountInactive => available.Count;
    public int CountAll      => CountActive + CountInactive;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
            available.Enqueue(CreateInstance());
    }

    /// <summary>Returns a ready-to-use object from the pool.</summary>
    public T Get()
    {
        T obj = available.Count > 0 ? available.Dequeue() : CreateInstance();
        obj.gameObject.SetActive(true);
        CountActive++;
        return obj;
    }

    /// <summary>Returns an object to the pool. Deactivates it automatically.</summary>
    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        available.Enqueue(obj);
        CountActive = Mathf.Max(0, CountActive - 1);
    }

    private T CreateInstance()
    {
        T instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(false);
        return instance;
    }
}
