using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityUIController : MonoBehaviour
{
    public Text SensitivityText;

    private float _previousSensitivity;

    void Update()
    {
        if (Mathf.Abs(_previousSensitivity - GameManager.Instance.SensitivityX) > 0.01f)
        {
            _previousSensitivity = GameManager.Instance.SensitivityX;
            SensitivityText.text = GameManager.Instance.SensitivityX.ToString("n1");
        }
    }
}
