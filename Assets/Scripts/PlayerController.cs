using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public properties
    public GameObject HookPrefab;
    public LayerMask HookRaycastLayerMask;

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
    }

    void Update()
    {
        // Mouse look

        float rotationX = CameraRoot.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * GameManager.Instance.SensitivityX;

        // Making clampf work properly
        rotationX = Mathf.Clamp(rotationX <= 180 ? rotationX : -360 + rotationX, -180f, 180f);

        _rotationY += Input.GetAxisRaw("Mouse Y") * GameManager.Instance.SensitivityY;
        _rotationY = Mathf.Clamp(_rotationY <= 180 ? _rotationY : -360 + _rotationY, -89f, 89f);

        CameraRoot.transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0f);

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

    private void SetHookLineRendererPositions()
    {
        if (!_hookActive) return;

        HookLineRenderer.enabled = true;

        HookLineRenderer.SetPosition(0, HookLineSource.position);
        HookLineRenderer.SetPosition(1, _hook.transform.position);
    }
}
