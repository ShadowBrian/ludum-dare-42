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
    }
}
