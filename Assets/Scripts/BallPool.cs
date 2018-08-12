using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : GenericComponentPool<BallController>
{
    protected override void InitializeObject(BallController obj)
    {

    }

    protected override void ReuseObject(BallController obj)
    {
        obj.Reset();
        obj.gameObject.SetActive(true);
    }

    public void ResetAll()
    {
        for (var i = 0; i < Pool.Count; i++)
        {
            Pool[i].gameObject.SetActive(false);
        }
    }
}
