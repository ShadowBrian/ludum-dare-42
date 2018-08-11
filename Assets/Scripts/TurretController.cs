using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform CannonAnchor;

    public float ShotInterval;
    public float ShotSpread;
    public float ShotForce;
    public float ShotForceRandomness;

    public float StickyChance;

    private float _shotTimer;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 1f / ShotInterval;
    }

    void Update()
    {
        var dt = Time.deltaTime;

        _shotTimer += dt;
        if (_shotTimer > ShotInterval)
        {
            _shotTimer = 0f;

            // Trigger animation
            _animator.SetTrigger("Shoot");

            var sticky = Random.Range(0f, 1f) < StickyChance;

            var shot = GameManager.Instance.BallPool.GetPooledObject();
            shot.component.Sticky = sticky;
            shot.gameObject.transform.position = CannonAnchor.transform.position;

            var direction = CannonAnchor.transform.forward;
            direction += Random.onUnitSphere * ShotSpread;
            var force = ShotForce + Random.Range(-ShotForceRandomness, ShotForceRandomness);

            shot.component.RigidBody.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }
}
