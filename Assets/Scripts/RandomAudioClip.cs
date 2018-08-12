using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioClip : MonoBehaviour
{
    public List<AudioClip> Clips;

    private AudioSource _source;

    private bool _played;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_source.isPlaying)
        {
            return;
        }
        else
        {
            if (_played)
            {
                _played = false;
                _source.Stop();
                gameObject.SetActive(false);
            }
            else
            {
                _source.clip = Clips[Random.Range(0, Clips.Count)];
                _source.Play();
                _played = true;
            }
        }
    }
}