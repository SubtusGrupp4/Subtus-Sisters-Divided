using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [Header("Instantiated Players")]
    public Transform playerTop;
    public Transform playerBot;

    [Header("Prefab Players")]
    public GameObject playerTopPrefab;
    public GameObject playerBotPrefab;

    [Header("Game Variables")]
    public bool onePlayerDead = false;
    [SerializeField]
    private Transform DialogueManager;

    [Header("Screen")]
    [SerializeField]
    private int resolutionX = 1920;
    [SerializeField]
    private int resolutionY = 1080;
    [Header("Keys")]
    [SerializeField]
    private KeyCode resetKey;
    [SerializeField]
    private KeyCode pauseKey;
    [SerializeField]
    private KeyCode killKey;

    [Header("Pausing")]
    [SerializeField]
    private Canvas pauseMenu;
    public bool isPaused = false;
    public float fillTime;
    [SerializeField]
    private float player1X = 0f;
    [SerializeField]
    private float player2X = 0f;
    private float fillRate = 1f;
    private float emptyRate = 0.4f;

    [SerializeField]
    [FMODUnity.EventRef]
    private string pauseEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    private string unpauseEvent;

    [SerializeField]
    private RadialProgressBar radial1;
    [SerializeField]
    private RadialProgressBar radial2;

    private void Awake()
    {
        CreateSingleton();
        SetStartResolution(resolutionX, resolutionY);
    }

    private void Start()
    {
        if (playerTop == null)
        {
            Debug.LogError("PlayerTop not assigned on Start() in GameManager!");
            playerTop = GameObject.Find("PlayerTop").transform;
        }
        if (playerBot == null)
        {
            Debug.LogError("PlayerBot not assigned on Start() in GameManager!");
            playerBot = GameObject.Find("PlayerBot").transform;
        }
        if (playerTopPrefab == null)
            Debug.LogError("PlayerTopPrefab not assigned on Start() in GameManager!");
        if (playerBotPrefab == null)
            Debug.LogError("PlayerBotPrefab not assigned on Start() in GameManager!");

        pauseMenu.gameObject.SetActive(false);
        onePlayerDead = false;

        DialogueManager.gameObject.SetActive(true);
    }


    private void Update()
    {
        RestartKey();
        PauseKey();
        KillKey();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            LevelManager.instance.ChangeLevel(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            LevelManager.instance.ChangeLevel(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            LevelManager.instance.ChangeLevel(2);

        if (isPaused)
            ReadQuit();

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
            LevelManager.instance.ResetLevel();
    }

    private void PauseKey()
    {
        if (Input.GetKeyDown(pauseKey) || Input.GetKeyDown("joystick button 7"))
        {
            if (isPaused)
                FMODUnity.RuntimeManager.PlayOneShot(unpauseEvent, transform.position);
            else
                FMODUnity.RuntimeManager.PlayOneShot(pauseEvent, transform.position);

            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
            // Use something else instead
            Time.timeScale = pauseMenu.gameObject.activeSelf ? 0f : 1f;
            isPaused = pauseMenu.gameObject.activeSelf;
        }
    }

    private void ReadQuit()
    {
        if (Input.GetKey("joystick 1 button 2"))
            player1X += fillRate * Time.unscaledDeltaTime;
        else
            player1X -= Time.unscaledDeltaTime * emptyRate;

        if (Input.GetKey("joystick 2 button 2"))
            player2X += fillRate * Time.unscaledDeltaTime;
        else
            player2X -= Time.unscaledDeltaTime * emptyRate;

        // Clamping to appropriate numbers
        player1X = Mathf.Clamp(player1X, 0.4f, 1f);
        player2X = Mathf.Clamp(player2X, 0.4f, 1f);

        // Applying the amount to each radial progress bar
        radial1.SetAmount(player1X);
        radial2.SetAmount(player2X);

        if (player1X >= 0.99f && player2X >= 0.99f)
        {
            Application.Quit();
        }
    }

    private void KillKey()
    {
        if (Input.GetKeyDown(killKey))
        {
            playerTop.GetComponent<Reviving>().Die();
            onePlayerDead = true;
            playerBot.GetComponent<Reviving>().Die();
        }
    }

    public float PreventZero(float value)
    {
        value = value < 0.01f ? 0.01f : value;
        return value;
    }

    public float PreventZero(float value, float min)
    {
        value = value < min ? min : value;
        return value;
    }

    public float PreventZero(float value, float min, float newValue)
    {
        value = value < min ? newValue : value;
        return value;
    }
}
