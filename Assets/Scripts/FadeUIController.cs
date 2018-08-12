using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUIController : MonoBehaviour
{
    public Image FadeImage;

    public void SetFade(float t)
    {
        FadeImage.color = new Color(0f, 0f, 0f, t);
    }
}
