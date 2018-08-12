using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public properties
    public GameObject HookPrefab;
    public LayerMask HookRaycastLayerMask;
    public LayerMask MenuLayerMask;

    public float HookMaxPullForce;
    public float HookMinPullForce;
    public float HookPullMaxDistance;
    public float HookPullMinDistance;

    // Self-references
    public GameObject CameraRoot;
    public LineRenderer HookLineRenderer;
    public Transform HookLineSource;

    private Rigidbody _rb;
    private float _rotationY;
    private GameObject _hook;

    private bool _hookActive;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _hook = Instantiate(HookPrefab);
        _hook.SetActive(false);

        HookLineRenderer.useWorldSpace = true;
        HookLineRenderer.enabled = false;

        _rb = GetComponent<Rigidbody>();

        if (GameManager.Instance.CurrentScene == "menu")
        {
            HookMaxPullForce /= 2f;
            HookMinPullForce /= 2f;
        }
    }

    void Update()
    {
        // Mouse look

        if (GameManager.Instance.EnableMouseLook)
        {
            float rotationX = CameraRoot.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * GameManager.Instance.SensitivityX;

            // Making clampf work properly
            rotationX = Mathf.Clamp(rotationX <= 180 ? rotationX : -360 + rotationX, -180f, 180f);

            _rotationY += Input.GetAxisRaw("Mouse Y") * GameManager.Instance.SensitivityY;
            _rotationY = Mathf.Clamp(_rotationY <= 180 ? _rotationY : -360 + _rotationY, -89f, 89f);

            CameraRoot.transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0f);
        }

        if (GameManager.Instance.InMenu)
        {
            HandleMenu();
        }

        if (!GameManager.Instance.Alive || GameManager.Instance.ExitingLevel || GameManager.Instance.InMenu) return;

        // Hook input
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, CameraRoot.transform.forward, out hit, 100f, HookRaycastLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.CompareTag("HookableSurface"))
                {
                    _hook.transform.position = hit.point;
                    _hook.SetActive(true);
                    _hookActive = true;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_hookActive)
            {
                _hookActive = false;
                _hook.SetActive(false);
                HookLineRenderer.enabled = false;
            }
        }

        // Hook line renderer
        SetHookLineRendererPositions();
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.Alive || GameManager.Instance.ExitingLevel) return;

        if (GameManager.Instance.MenuHookVisible && GameManager.Instance.CurrentScene == "menu")
        {
            _hookActive = true;
            _hook.SetActive(true);
            SetHookLineRendererPositions();
            _hook.transform.position = GameManager.Instance.MenuHookPosition.position;
        }

        var dt = Time.fixedDeltaTime;

        // Pull the player towards the hook
        if (_hookActive)
        {
            var towardsHook = (_hook.transform.position - transform.position).normalized;
            var distance = Vector3.Distance(_hook.transform.position, transform.position);
            var pullForce = Mathf.Clamp(distance, HookPullMinDistance, HookPullMaxDistance).Remap(HookPullMinDistance, HookPullMaxDistance, HookMinPullForce, HookMaxPullForce);

            _rb.AddForce(towardsHook * pullForce, ForceMode.Force);
        }
    }

    private void HandleMenu()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.MenuPlayPressed)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, CameraRoot.transform.forward, out hit, 100f, MenuLayerMask))
            {
                var option = hit.collider.gameObject.tag;
                if (option == "SensitivityDown")
                {
                    GameManager.Instance.SensitivityX -= 0.1f;
                    GameManager.Instance.SensitivityY -= 0.1f;
                }
                else if (option == "SensitivityUp")
                {
                    GameManager.Instance.SensitivityX += 0.1f;
                    GameManager.Instance.SensitivityY += 0.1f;
                }
                else if (option == "Play")
                {
                    GameManager.Instance.MenuPlayPressed = true;
                }
                else if (option == "Exit")
                {
                    Application.Quit();
                }

                GameManager.Instance.SensitivityX = Mathf.Clamp(GameManager.Instance.SensitivityX, 0.1f, 5f);
                GameManager.Instance.SensitivityY = Mathf.Clamp(GameManager.Instance.SensitivityY, 0.1f, 5f);

                PlayerPrefs.SetFloat("SensitivityX", GameManager.Instance.SensitivityX);
                PlayerPrefs.SetFloat("SensitivityY", GameManager.Instance.SensitivityY);
            }
        }
    }

    private void SetHookLineRendererPositions()
    {
        if (!_hookActive) return;

        HookLineRenderer.enabled = true;

        HookLineRenderer.SetPosition(0, HookLineSource.position);
        HookLineRenderer.SetPosition(1, _hook.transform.position);
    }

    void OnCollisionEnter(Collision other)
    {
        if (GameManager.Instance.ExitingLevel) return;

        if (other.collider.CompareTag("Obstacle"))
        {
            // Get the point of collision on the player's forward plane, make screen effect
            GameManager.Instance.Death();
            HookLineRenderer.enabled = false;
            _hookActive = false;
        }
    }
}
