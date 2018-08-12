using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 RotationAxis;
    public float RotationSpeed;

    public float RotationScale = 1f;

    private float t;

    void Awake()
    {

    }

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        t += dt;

        transform.localRotation = Quaternion.AngleAxis(t * RotationSpeed * RotationScale, RotationAxis);
    }
}
