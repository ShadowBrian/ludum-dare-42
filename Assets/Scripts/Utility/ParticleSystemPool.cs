using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : GenericObjectPool
{
    protected override void InitializeObject(GameObject obj)
    {

    }

    protected override void ReuseObject(GameObject obj)
    {
        obj.SetActive(true);
    }
}