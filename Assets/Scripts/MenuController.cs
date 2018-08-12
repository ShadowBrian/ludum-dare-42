using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Text PlayText;
    public Text ExitText;
    public BoxCollider PlayCollider;
    public BoxCollider ExitCollider;
    public GameObject Door;
    public float DoorAppearSpeed;
    public Transform MenuHookAnchor;

    private float _doorTimer;
    private float _initialDoorY;
    private Color _initialTextColor;

    void Awake()
    {
        GameManager.Instance.MenuPlayPressed = false;
        GameManager.Instance.MenuHookVisible = false;
        _initialDoorY = Door.transform.position.y;
        _initialTextColor = PlayText.color;
        GameManager.Instance.MenuHookPosition = MenuHookAnchor;
    }

    void Update()
    {
        if (GameManager.Instance.MenuPlayPressed)
        {
            if (Door.transform.position.y < -1f)
            {
                Door.transform.position += Time.deltaTime * Vector3.up * DoorAppearSpeed;
            }
            else
            {
                _doorTimer += Time.deltaTime;
            }

            if (_doorTimer > 0.75f)
            {
                GameManager.Instance.MenuHookVisible = true;
            }

            var fadeT = 1f - Mathf.Clamp(Door.transform.position.y.Remap(_initialDoorY, -1f, 0f, 1f) * 1.25f, 0f, 1f);
            var fadeColor = new Color(_initialTextColor.r, _initialTextColor.g, _initialTextColor.b, fadeT);
            PlayText.color = fadeColor;
            ExitText.color = fadeColor;
            PlayCollider.tag = "Untagged";
            ExitCollider.tag = "Untagged";
        }
    }
}
