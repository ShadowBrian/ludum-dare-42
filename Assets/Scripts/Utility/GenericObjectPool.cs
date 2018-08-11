using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool : MonoBehaviour
{
    public GameObject PooledObjectPrefab;
    public bool WillGrow;

    public List<GameObject> Pool;

    public GameObject Container;

    protected virtual void Awake()
    {
        Pool = new List<GameObject>();
    }

    /// <summary>
    /// Initialization of a newly spawned object
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void InitializeObject(GameObject obj)
    {

    }

    /// <summary>
    /// The object is being reused, reinitialize
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void ReuseObject(GameObject obj)
    {

    }

    /// <summary>
    /// Return an instance of the prefab the pool is used to handle
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            var pooledObject = Pool[i];
            if (pooledObject == null)
            {
                GameObject obj = InstantiateNew();
                Pool[i] = obj;
                return Pool[i];
            }
            if (!pooledObject.activeInHierarchy)
            {
                ReuseObject(pooledObject);
                return pooledObject;
            }
        }

        if (WillGrow)
        {
            GameObject obj = InstantiateNew();
            Pool.Add(obj);
            return obj;
        }

        return null;
    }

    private GameObject InstantiateNew()
    {
        GameObject obj = (GameObject)Instantiate(PooledObjectPrefab);
        InitializeObject(obj);

        if (Container != null)
        {
            obj.transform.parent = Container.transform;
        }

        return obj;
    }
}