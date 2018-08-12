using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform CannonAnchor;

    public float ShotInterval;
    public float ShotSpread;
    public float ShotForce;
    public float ShotForceRandomness;

    public float StickyChance;
    public float BallSize;

    public bool ActiveAtStart;
    public float SpinUpSpeed;

    public int MaxShots;

    private float _shotTimer;
    private Animator _animator;
    private bool _active;
    private float _spinUpSpeed;

    private List<Rotator> _rotators;
    private int _shotsLeft;

    public bool Silent;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 1f / ShotInterval;

        _spinUpSpeed = 0f;

        // Get rotators for spin up acceleration
        _rotators = GetComponentsInChildren<Rotator>().ToList();

        _shotsLeft = MaxShots;

        if (ActiveAtStart)
        {
            _active = true;
        }
    }

    void Update()
    {
        var dt = Time.deltaTime;

        if (!_active || !GameManager.Instance.Alive) return;

        // Set the rotators to spin up
        for (var i = 0; i < _rotators.Count; i++)
        {
            _rotators[i].RotationScale = _spinUpSpeed;
        }

        if (_spinUpSpeed < 1f)
        {
            _spinUpSpeed += dt * 1f / SpinUpSpeed;
            return;
        }
        else
        {
            _spinUpSpeed = 1f;
        }

        if (_shotsLeft == 0) return;

        _shotTimer += dt;
        if (_shotTimer > ShotInterval)
        {
            _shotTimer = 0f;
            _shotsLeft -= 1;

            if (!Silent)
            {
                var sfx = GameManager.Instance.TurretShootSFXPool.GetPooledObject();
                sfx.transform.position = transform.position;
                sfx.SetActive(true);
            }

            // Trigger animation
            _animator.SetTrigger("Shoot");

            var sticky = Random.Range(0f, 1f) < StickyChance;

            var shot = GameManager.Instance.BallPool.GetPooledObject();
            shot.component.Sticky = sticky;
            shot.component.Size = BallSize;
            shot.gameObject.transform.position = CannonAnchor.transform.position;

            var direction = CannonAnchor.transform.forward;
            direction += Random.onUnitSphere * ShotSpread;
            var force = ShotForce + Random.Range(-ShotForceRandomness, ShotForceRandomness);

            shot.component.RigidBody.AddForce(direction.normalized * force, ForceMode.Impulse);
            shot.component.Shoot();
        }
    }

    public void Activate()
    {
        _active = true;
    }
}
