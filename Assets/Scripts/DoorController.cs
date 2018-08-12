using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string TargetScene;

    public MeshRenderer FrameMR;
    public MeshRenderer InnerMR;

    public bool EndsGame;

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.ExitingLevel) return;

        if (other.CompareTag("Player"))
        {
            if (EndsGame)
            {
                GameManager.Instance.GameEnded = true;
            }

            FrameMR.material.color = Color.white;
            InnerMR.material.SetColor("_EffectColor", Color.white);
            StartCoroutine(GameManager.Instance.ReachExit(TargetScene));
        }
    }
}
