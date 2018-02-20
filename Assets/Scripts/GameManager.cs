using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("Instantiated Players")]
    public Transform playerTop;
    public Transform playerBot;

    [Header("Prefab Players")]
    public GameObject playerTopPrefab;
    public GameObject playerBotPrefab;

    [Header("Game Variables")]
    public bool onePlayerDead = false;

    [Header("Screen")]
    [SerializeField]
    private int resolutionX = 1920;
    [SerializeField]
    private int resolutionY = 1080;
    [Header("Keys")]
    [SerializeField]
    private KeyCode resetKey;
    [SerializeField]
    private KeyCode quitKey;

    public bool isFrozen = false;

    private void Awake()
    {
        CreateSingleton();
        SetStartResolution(resolutionX, resolutionY);
    }

    private void Start()
    {
        if (playerTop == null)
            Debug.LogError("PlayerTop not assigned on Start() in GameManager!");
        if (playerBot == null)
            Debug.LogError("PlayerBot not assigned on Start() in GameManager!");
        if (playerTopPrefab == null)
            Debug.LogError("PlayerTopPrefab not assigned on Start() in GameManager!");
        if (playerBotPrefab == null)
            Debug.LogError("PlayerBotPrefab not assigned on Start() in GameManager!");
    }

    private void Update () {
        RestartKey();
        QuitKey();
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void SetStartResolution(int resX, int resY)
    {
        Screen.SetResolution(resX, resY, true);
    }

    private void RestartKey()
    {
        if (Input.GetKeyDown(resetKey))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitKey()
    {
        if (Input.GetKeyDown(quitKey))
            Application.Quit();
    }

    public void SetFreezeGame(bool freeze)
    {
        isFrozen = freeze;
    }

    public float PreventDividingByZero(float value) 
    {
        value = value < 0.01f ? 0.01f : value;
        return value;
    }

    public float PreventDividingByZero(float value, float min)
    {
        value = value < min ? min : value;
        return value;
    }
}
