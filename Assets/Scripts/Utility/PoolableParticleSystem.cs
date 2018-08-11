using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolableParticleSystem : MonoBehaviour
{
    private ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!_ps.IsAlive())
        {
            _ps.Clear();
            gameObject.SetActive(false);
            return;
        }

        if (!_ps.isPlaying)
        {
            _ps.Play();
        }
    }
}