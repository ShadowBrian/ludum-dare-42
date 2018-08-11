using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody RigidBody;

    [HideInInspector]
    public bool Sticky;

    public float MakeSolidWhenStillTime;

    private bool _solid;
    private Vector3 _oldPosition;
    private float _solidTimer;

    void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        MakeSolid(false);
        _solidTimer = 0f;
    }

    void Update()
    {
        if (_solid || Sticky) return;

        if (Vector3.Distance(transform.position, _oldPosition) > 0.01f)
        {
            _solidTimer = 0f;
            _oldPosition = transform.position;
        }

        _solidTimer += Time.deltaTime;

        if (_solidTimer > MakeSolidWhenStillTime)
        {
            MakeSolid(true);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (_solid || !Sticky) return;

        if (other.collider.gameObject.CompareTag("Obstacle") || other.collider.gameObject.CompareTag("HookableSurface"))
        {
            MakeSolid(true);
        }
    }

    private void MakeSolid(bool value)
    {
        RigidBody.useGravity = !value;
        RigidBody.isKinematic = value;
        _solid = value;
    }
}
