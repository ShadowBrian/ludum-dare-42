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
    public GameObject RefBallPool;
    public GenericComponentPool<BallController> BallPool;

    // Global properties
    public float SensitivityX;
    public float SensitivityY;

    void Awake()
    {
        // Setup singleton
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _instance = this;

        // Pool references
        BallPool = RefBallPool.GetComponent<GenericComponentPool<BallController>>();
    }

    public void ChangeLevel(string sceneName)
    {

    }
}