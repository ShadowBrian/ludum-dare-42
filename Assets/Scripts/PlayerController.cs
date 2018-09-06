using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

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
    public SteamVR_TrackedObject[] Hands = new SteamVR_TrackedObject[2];
    public SteamVR_Controller.Device[] controllerinputs = new SteamVR_Controller.Device[2];

    public LineRenderer HookLineRenderer;
    public Transform HookLineSource;

    public Transform Collider,Head;


    private Rigidbody _rb;
    private float _rotationY;
    private GameObject _hook;

    private bool _hookActive;

    void Awake()
    {
        //Lol screw cursor locking in VR
        //Cursor.lockState = CursorLockMode.Locked;

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

        if (GameManager.Instance.GameEnded)
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }

    bool GotHands;

    IEnumerator Start() {

        //Wait for both hands to be turned on.
        while(Hands[0].index == SteamVR_TrackedObject.EIndex.None) {
            yield return null;
        }

        while (Hands[1].index == SteamVR_TrackedObject.EIndex.None) {
            yield return null;
        }
        controllerinputs = new SteamVR_Controller.Device[2];

        //Get their input references.
        controllerinputs[0] = SteamVR_Controller.Input((int)Hands[0].index);
        controllerinputs[1] = SteamVR_Controller.Input((int)Hands[1].index);
        GotHands = true;
    }

    void Update()
    {
        // Mouse look- Completely not needed for VR :)

        /*if (GameManager.Instance.EnableMouseLook)
        {
            float rotationX = CameraRoot.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * GameManager.Instance.SensitivityX;

            // Making clampf work properly
            rotationX = Mathf.Clamp(rotationX <= 180 ? rotationX : -360 + rotationX, -180f, 180f);

            _rotationY += Input.GetAxisRaw("Mouse Y") * GameManager.Instance.SensitivityY;
            _rotationY = Mathf.Clamp(_rotationY <= 180 ? _rotationY : -360 + _rotationY, -89f, 89f);

            CameraRoot.transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0f);
        }*/

        //Don't do anything if you have no hands on.
        if (!GotHands) {
            return;
        }

        Collider.localPosition = new Vector3(Head.localPosition.x, 0, Head.localPosition.z);

        if (GameManager.Instance.InMenu)
        {
            HandleMenu();
        }

        if (!GameManager.Instance.Alive || GameManager.Instance.ExitingLevel || GameManager.Instance.InMenu) return;

        // Hook input
        //Trigger as fire
        if (controllerinputs[0].GetHairTriggerDown())
        {
            RaycastHit hit;
            //Raycast from hand
            if (Physics.Raycast(transform.position, Hands[0].transform.forward - Hands[0].transform.up, out hit, 100f, HookRaycastLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.CompareTag("HookableSurface"))
                {

                    if (_hookActive) {
                        _hookActive = false;
                        _hook.SetActive(false);
                        HookLineRenderer.enabled = false;
                    }

                    _hook.transform.position = hit.point;
                    _hook.SetActive(true);
                    _hookActive = true;
                    HookLineSource = Hands[0].transform;

                    var sfx = GameManager.Instance.HookSFXPool.GetPooledObject();
                    sfx.SetActive(true);
                }
            }
        }

        //Grip as let go
        if (controllerinputs[0].GetPressDown(EVRButtonId.k_EButton_Grip))
        {
            if (_hookActive)
            {
                _hookActive = false;
                _hook.SetActive(false);
                HookLineRenderer.enabled = false;
            }
        }

        // Hook input2
        //Trigger as fire
        if (controllerinputs[1].GetHairTriggerDown()) {
            RaycastHit hit;
            //Raycast from hand
            if (Physics.Raycast(transform.position, Hands[1].transform.forward - Hands[1].transform.up, out hit, 100f, HookRaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                if (hit.collider.gameObject.CompareTag("HookableSurface")) {

                    if (_hookActive) {
                        _hookActive = false;
                        _hook.SetActive(false);
                        HookLineRenderer.enabled = false;
                    }

                    _hook.transform.position = hit.point;
                    _hook.SetActive(true);
                    _hookActive = true;
                    HookLineSource = Hands[1].transform;
                    var sfx = GameManager.Instance.HookSFXPool.GetPooledObject();
                    sfx.SetActive(true);
                }
            }
        }

        //Grip as let go
        if (controllerinputs[1].GetPressDown(EVRButtonId.k_EButton_Grip)) {
            if (_hookActive) {
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
        if (controllerinputs[0].GetHairTriggerDown() && !GameManager.Instance.MenuPlayPressed)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Hands[0].transform.forward - Hands[0].transform.up, out hit, 100f, MenuLayerMask))
            {
                var option = hit.collider.gameObject.tag;
                if (option == "SensitivityDown")
                {
                    GameManager.Instance.SensitivityX -= 0.1f;
                    GameManager.Instance.SensitivityY -= 0.1f;

                    var sfx = GameManager.Instance.MenuSensSFXPool.GetPooledObject();
                    sfx.transform.position = transform.position;
                    sfx.SetActive(true);
                }
                else if (option == "SensitivityUp")
                {
                    GameManager.Instance.SensitivityX += 0.1f;
                    GameManager.Instance.SensitivityY += 0.1f;

                    var sfx = GameManager.Instance.MenuSensSFXPool.GetPooledObject();
                    sfx.transform.position = transform.position;
                    sfx.SetActive(true);
                }
                else if (option == "Play")
                {
                    GameManager.Instance.GameEnded = false;
                    GameManager.Instance.MenuPlayPressed = true;

                    var sfx = GameManager.Instance.MenuPlaySFXPool.GetPooledObject();
                    sfx.transform.position = transform.position;
                    sfx.SetActive(true);
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
        if (controllerinputs[1].GetHairTriggerDown() && !GameManager.Instance.MenuPlayPressed) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Hands[1].transform.forward - Hands[1].transform.up, out hit, 100f, MenuLayerMask)) {
                var option = hit.collider.gameObject.tag;
                if (option == "SensitivityDown") {
                    GameManager.Instance.SensitivityX -= 0.1f;
                    GameManager.Instance.SensitivityY -= 0.1f;

                    var sfx = GameManager.Instance.MenuSensSFXPool.GetPooledObject();
                    sfx.transform.position = transform.position;
                    sfx.SetActive(true);
                }
                else if (option == "SensitivityUp") {
                    GameManager.Instance.SensitivityX += 0.1f;
                    GameManager.Instance.SensitivityY += 0.1f;

                    var sfx = GameManager.Instance.MenuSensSFXPool.GetPooledObject();
                    sfx.transform.position = transform.position;
                    sfx.SetActive(true);
                }
                else if (option == "Play") {
                    GameManager.Instance.GameEnded = false;
                    GameManager.Instance.MenuPlayPressed = true;

                    var sfx = GameManager.Instance.MenuPlaySFXPool.GetPooledObject();
                    sfx.transform.position = transform.position;
                    sfx.SetActive(true);
                }
                else if (option == "Exit") {
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

        if (other.collider.CompareTag("Obstacle") && GameManager.Instance.Alive)
        {
            // Get the point of collision on the player's forward plane, make screen effect
            GameManager.Instance.Death();
            HookLineRenderer.enabled = false;
            _hookActive = false;

            var sfx = GameManager.Instance.DeathSFXPool.GetPooledObject();
            sfx.transform.position = transform.position;
            sfx.SetActive(true);
        }
        else
        {
            var sfx = GameManager.Instance.WallHitSFXPool.GetPooledObject();
            sfx.transform.position = transform.position;
            sfx.SetActive(true);
        }
    }
}
