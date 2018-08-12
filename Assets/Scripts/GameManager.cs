using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    // Object pools
    public BallPool BallPool;
    public GenericObjectPool MenuPlaySFXPool;
    public GenericObjectPool MenuSensSFXPool;
    public GenericObjectPool TurretShootSFXPool;
    public GenericObjectPool HookSFXPool;
    public GenericObjectPool WallHitSFXPool;
    public GenericObjectPool DeathSFXPool;
    public GenericObjectPool DoorSFXPool;

    // Global properties
    public float SensitivityX;
    public float SensitivityY;

    public bool EnableMouseLook = true;
    public bool Alive = true;
    public bool ExitingLevel;

    public float DeathTime;
    public float DeathFadeTime;
    public float SpawnFadeTime;
    private float _deathTimer;
    private float _previousTime;

    [HideInInspector]
    public string CurrentScene;

    [HideInInspector]
    public bool LoadingScene;

    [HideInInspector]
    public float TimeScale;

    private float _initialFixedDeltaTime;

    public GameObject CrosshairUI;
    public GameObject FadeUI;

    public float SceneExitTime;

    public bool MenuPlayPressed;
    public bool MenuHookVisible;
    public Transform MenuHookPosition;
    public bool GameEnded;

    private FadeUIController _fadeUIController;
    private float _fadeAmount;
    private float _fadeTarget;
    private float _fadeTimer;
    private float _fadeTime;
    private float _fadeStart;
    private float _fadeStartTimer;
    private float _fadeNextTime;
    private float _fadeNextTarget;

    public bool InMenu;

    void Awake()
    {
        // Update scene name for reloading purposes
        if (_instance != null)
        {
            _instance.CurrentScene = SceneManager.GetActiveScene().name;
        }
        else
        {
            CurrentScene = SceneManager.GetActiveScene().name;
        }

        // Setup singleton
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Get sensitivity settings
        var prefSensX = PlayerPrefs.GetFloat("SensitivityX");
        var prefSensY = PlayerPrefs.GetFloat("SensitivityY");
        SensitivityX = prefSensX > 0f ? prefSensX : 1f;
        SensitivityY = prefSensY > 0f ? prefSensY : 1f;

        _initialFixedDeltaTime = Time.fixedDeltaTime;

        DontDestroyOnLoad(gameObject);
        _instance = this;

        // Create UIs
        var crosshairUI = Instantiate(CrosshairUI);
        crosshairUI.transform.parent = transform;

        var fadeUI = Instantiate(FadeUI);
        fadeUI.transform.parent = transform;
        _fadeUIController = fadeUI.GetComponent<FadeUIController>();
        _fadeUIController.SetFade(0f);
    }

    void Update()
    {
        // Ending the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (LoadingScene) return;

        InMenu = CurrentScene == "menu";

        // Custom dt because of slowdown effect on death anim
        var dt = Time.realtimeSinceStartup - _previousTime;
        _previousTime = Time.realtimeSinceStartup;

        if (!Alive)
        {
            _deathTimer += dt;

            // Slowdown the game for DeathTime seconds
            var t = Mathf.Clamp(1f - _deathTimer / DeathTime, 0f, 1f);
            Time.timeScale = t;
            Time.fixedDeltaTime = Mathf.Clamp(_initialFixedDeltaTime * t, 0.005f, 1f);

            if (_deathTimer > DeathTime + DeathFadeTime)
            {
                _deathTimer = 0;
                ChangeLevel(CurrentScene);
            }
        }

        // Check if we need to start fading
        if (_fadeStartTimer > 0f)
        {
            _fadeStartTimer -= dt;

            if (_fadeStartTimer <= 0f)
            {
                _fadeStartTimer = 0f;
                Fade(_fadeNextTime, _fadeNextTarget);
            }
        }

        // Fading
        if (_fadeTimer > 0f)
        {
            _fadeTimer -= dt;

            // Fade the screen out in DeathFadeTime seconds
            var fadeT = 1f - _fadeTimer / _fadeTime;
            _fadeAmount = Mathf.Lerp(_fadeStart, _fadeTarget, fadeT);

            _fadeUIController.SetFade(_fadeAmount);

            if (_fadeTimer <= 0f)
            {
                _fadeTimer = 0f;
                _fadeAmount = _fadeTarget;
            }
        }
    }

    public void Death()
    {
        if (!Alive) return;

        Alive = false;
        EnableMouseLook = false;
        StartFadeIn(DeathTime, DeathFadeTime, 1f);
    }

    public IEnumerator ReachExit(string nextScene)
    {
        ExitingLevel = true;
        Fade(SceneExitTime, 1f);
        yield return new WaitForSeconds(SceneExitTime);
        ChangeLevel(nextScene);
    }

    public void ChangeLevel(string sceneName)
    {
        BallPool.ResetAll();

        Time.timeScale = 1f;
        Time.fixedDeltaTime = _initialFixedDeltaTime;

        LoadingScene = true;
        SceneManager.LoadScene(sceneName);
        LoadingScene = false;

        Fade(SpawnFadeTime, 0f);

        ExitingLevel = false;
        EnableMouseLook = true;
        Alive = true;
        Time.timeScale = 1f;
    }

    public void Fade(float time, float target)
    {
        _fadeTime = time;
        _fadeTimer = time;
        _fadeTarget = target;
        _fadeStart = _fadeAmount;
    }

    // Not a coroutine, because timeScale affects those too
    public void StartFadeIn(float offset, float time, float target)
    {
        _fadeStartTimer = offset;
        _fadeNextTime = time;
        _fadeNextTarget = target;
    }
}