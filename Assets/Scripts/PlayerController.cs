using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _rotationY;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Mouse look

        float rotationX = transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * GameManager.Instance.SensitivityX;

        // Making clampf work properly
        rotationX = Mathf.Clamp(rotationX <= 180 ? rotationX : -360 + rotationX, -180f, 180f);

        _rotationY += Input.GetAxisRaw("Mouse Y") * GameManager.Instance.SensitivityY;
        _rotationY = Mathf.Clamp(_rotationY <= 180 ? _rotationY : -360 + _rotationY, -89f, 89f);

        transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0f);
    }
}
