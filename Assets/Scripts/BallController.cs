using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody RigidBody;

    public bool Sticky;

    [HideInInspector]
    public float Size = 1f;

    public float MakeSolidWhenStillTime;
    public float ExpandTime;
    public float ExpandScale;
    public float ExpandScaleRandomness;
    public float SpawnTime;

    private bool _solid;
    private Vector3 _oldPosition;
    private float _solidTimer;

    private bool _expanding;
    private float _expandingTimer;
    private Vector3 _initialScale;
    private float _initialMass;
    private Vector3 _targetScale;
    private Vector3 _expandLocation;

    private bool _spawning;
    private float _spawnTimer;

    void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();

        _initialScale = transform.localScale;
        _initialMass = RigidBody.mass;
        MakeTargetScale();
    }

    public void Reset()
    {
        MakeSolid(false);
        _solidTimer = 0f;
        RigidBody.mass = _initialMass;

        _spawning = false;
        _spawnTimer = 0f;
        _expanding = false;
        _expandingTimer = 0f;
        MakeTargetScale();
    }

    public void Shoot()
    {
        _spawning = true;
        transform.localScale = Vector3.zero;
        _initialScale = Vector3.one * Size;
    }

    void Update()
    {
        if (_solid || Sticky) return;

        var dt = Time.deltaTime;

        if (_spawning)
        {
            _spawnTimer += dt;
            var spawnT = Mathf.Clamp(_spawnTimer / SpawnTime, 0f, 1f);
            transform.localScale = _initialScale * spawnT;

            if (_spawnTimer > SpawnTime)
            {
                _spawning = false;
                transform.localScale = _initialScale;
            }
        }

        if (Vector3.Distance(transform.position, _oldPosition) > 0.01f)
        {
            _expandingTimer = 0f;
            _oldPosition = transform.position;
        }

        _expandingTimer += dt;

        if (_expandingTimer > ExpandTime && !_expanding)
        {
            _expandLocation = transform.position;
            _expanding = true;
        }

        if (_expanding)
        {
            _solidTimer += dt;

            var t = Mathf.Clamp(_solidTimer / MakeSolidWhenStillTime, 0f, 1f);
            RigidBody.mass = Mathf.Lerp(_initialMass * Size, Mathf.Pow(_initialMass * Size * ExpandScale, 3), t);
            transform.localScale = Vector3.Lerp(_initialScale, _targetScale, t);

            if (_solidTimer > MakeSolidWhenStillTime)
            {
                MakeSolid(true);
            }
        }
    }

    void LateUpdate()
    {
        if (!_expanding || _solid) return;

        if (Vector3.Distance(_expandLocation, transform.position) > 0.5f)
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

        this.enabled = !value;
    }

    private void MakeTargetScale()
    {
        _targetScale = Vector3.one * (ExpandScale + Random.Range(-ExpandScaleRandomness, ExpandScaleRandomness));
    }
}
