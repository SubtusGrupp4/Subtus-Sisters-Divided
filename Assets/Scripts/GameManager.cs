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

    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Slider slider;
    public Text progressText;


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
            ChangeLevel(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeLevel(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeLevel(2);

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
            ResetLevel();
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
        {
            player1X += fillRate * Time.unscaledDeltaTime;
        }
        else if (player1X > 0)
            player1X -= Time.unscaledDeltaTime * emptyRate;

        if (Input.GetKey("joystick 2 button 2"))
        {
            player2X += fillRate * Time.unscaledDeltaTime;
        }
        else if (player2X > 0)
            player2X -= Time.unscaledDeltaTime * emptyRate;

        if (player1X >= fillTime && player2X >= fillTime)
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

    public void ResetLevel()
    {
        ChangeLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeLevel(int id)
    {
        GameObject[] gos = FindObjectsOfType<GameObject>();
        foreach (GameObject go in gos)
        {
            FMODEmitter[] emitters = go.GetComponents<FMODEmitter>();
            foreach (FMODEmitter emitter in emitters)
                emitter.Stop();
        }

        StartCoroutine(LevelLoadAsynchronous(id));
    }

    IEnumerator LevelLoadAsynchronous(int id)
    {
        float progress;
        AsyncOperation operation = SceneManager.LoadSceneAsync(id);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(progress);

            slider.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
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
