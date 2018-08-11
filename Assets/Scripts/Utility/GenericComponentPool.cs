using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Component<T>
{
    public T component;
    public GameObject gameObject;
}

public class GenericComponentPool<T> : MonoBehaviour
{
    public GameObject PooledObjectPrefab;
    public bool WillGrow;

    public List<Component<T>> Pool;

    public GameObject Container;

    protected virtual void Awake()
    {
        Pool = new List<Component<T>>();
    }

    /// <summary>
    /// Initialization of a newly spawned object
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void InitializeObject(T obj)
    {

    }

    /// <summary>
    /// The object is being reused, reinitialize
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void ReuseObject(T obj)
    {

    }

    /// <summary>
    /// Return an instance of the prefab the pool is used to handle
    /// </summary>
    /// <returns></returns>
    public Component<T> GetPooledObject()
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            var pooledObject = Pool[i];
            if (pooledObject == null)
            {
                Component<T> comp = InstantiateNew();
                Pool[i] = comp;
                return Pool[i];
            }
            if (!pooledObject.gameObject.activeInHierarchy)
            {
                ReuseObject(pooledObject.component);
                return pooledObject;
            }
        }

        if (WillGrow)
        {
            Component<T> comp = InstantiateNew();
            Pool.Add(comp);
            return comp;
        }

        return null;
    }

    private Component<T> InstantiateNew()
    {
        GameObject obj = (GameObject)Instantiate(PooledObjectPrefab);

        if (Container != null)
        {
            obj.transform.parent = Container.transform;
        }

        var comp = new Component<T>();
        comp.component = obj.GetComponent<T>();
        comp.gameObject = obj;

        InitializeObject(comp.component);

        return comp;
    }
}
