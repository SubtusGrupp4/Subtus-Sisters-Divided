using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Transform playerTop;
    public Transform playerBot;

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
