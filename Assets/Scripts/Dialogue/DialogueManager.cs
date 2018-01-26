using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private AudioSource[] audioSources;
    private Queue<string> sentences;

    [HideInInspector]
    public bool isBusy = false;
    [HideInInspector]
    public bool moveCamera = false;

    [Tooltip("Middle Dialogue Canvas. The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
    [SerializeField]
    private Canvas mDialogueCanvas;

    [Tooltip("Right Dialogue Canvas. The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
    [SerializeField]
    private Canvas rDialogueCanvas;

    [Tooltip("Left Dialogue Canvas. The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
    [SerializeField]
    private Canvas lDialogueCanvas;

    private TextMeshProUGUI nameText;
    private Image image;
    private TextMeshProUGUI dialogueText;

    private int di = 0;
    private int sentenceIndex = 0;
    private Dialogue[] dialogues;

    private int charIndex = 0;
    private string sentenceToWrite = string.Empty;
    private string writtenSentence = string.Empty;

    private CameraController camController;

    [HideInInspector]
    public bool freezeCamera = false;

    [Header("Typing Settings")]
    [Tooltip("The global speed in hundreds of a second for each character to appear. Can be overrided on individual Dialogue components.")]
    [SerializeField]
    private float typeSpeed = 8f;
    private float actualTypeSpeed;
    private float typeTime = 0f;
    [SerializeField]
    private AudioClip[] typingSounds;
    [SerializeField]
    [Range(0f, 1f)]
    private float typingVolume = 1f;

    private Transform panel;

    private bool fadeInDone = false;

    private bool fadeOutDone = false;
    private bool doFadeOut = false;

    private void Awake()
    {
        CreateSingleton();
    }

    void Start ()
    {
        if (lDialogueCanvas == null || rDialogueCanvas == null)
            Debug.LogWarning("Missing references on DialogueManager. Assign in the inspector on the DialogueManager object.");


        sentences = new Queue<string>();
        mDialogueCanvas.enabled = false;
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        actualTypeSpeed = typeSpeed;

        camController = Camera.main.GetComponent<CameraController>();

        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length > 2)
            Debug.LogError("DialogueManager requires 2 AudioSource components to function.");
        audioSources[1].volume = typingVolume;
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void FetchDialogue(Dialogue[] dialogues)
    {
        this.dialogues = dialogues;

        StartDialogue();
    }

    private void Update()
    {
        if (dialogues != null)
        {
            switch (dialogues[di].playerIndex )
            {
                case 0:
                    // TODO: If any input
                    if(Input.GetKeyDown(KeyCode.E))
                        InputGet();
                    break;
                case 1:
                    // TODO: If input from Player 1
                    if (Input.GetKeyDown(KeyCode.E))
                        InputGet();
                    break;
                case 2:
                    // TODO: If input from Player 2
                    if (Input.GetKeyDown(KeyCode.E))
                        InputGet();
                    break;
            }
            if (sentenceToWrite.Length > charIndex)
            {
                if (dialogues[di].fadeIn && !fadeInDone)
                    return;

                if (typeTime < 1f)
                {
                    typeTime += Time.deltaTime / (actualTypeSpeed / 100f);
                }
                else
                {
                    char toWrite = sentenceToWrite[charIndex];

                    writtenSentence += toWrite;
                    dialogueText.text = writtenSentence;
                    charIndex++;
                    typeTime = 0f;
                    if (dialogues[di].typeSounds && toWrite != ' ')
                    {
                        int typeSoundIndex = Random.Range(0, typingSounds.Length);
                        audioSources[1].clip = typingSounds[typeSoundIndex];
                        audioSources[1].Play();
                    }
                }
            }

            if(dialogues != null && dialogues[di].fadeIn && !doFadeOut)
            {
                if (panel.GetComponent<CanvasGroup>().alpha < 1f)
                {
                    panel.GetComponent<CanvasGroup>().alpha += Time.deltaTime / dialogues[di].fadeTime;
                    fadeInDone = false;
                }
                else
                {
                    panel.GetComponent<CanvasGroup>().alpha = 1f;
                    if (!fadeInDone)
                    {
                        DisplayNextSentence();
                        fadeInDone = true;
                    }
                }
            }

            if(dialogues != null && dialogues[di].fadeOut && doFadeOut)
            {
                if (panel.GetComponent<CanvasGroup>().alpha > 0f)
                {
                    panel.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / dialogues[di].fadeTime;
                    fadeOutDone = false;
                }
                else
                {
                    panel.GetComponent<CanvasGroup>().alpha = 0f;
                    if (!fadeOutDone)
                    {
                        DisplayNextDialogue();
                        fadeOutDone = true;
                        doFadeOut = false;
                    }
                }
            }
        }
    }

    private void InputGet()
    {
        if(dialogues[di].fadeIn && panel.GetComponent<CanvasGroup>().alpha < 0.99f && !fadeInDone)
        {
            panel.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else if(dialogues[di].fadeOut && panel.GetComponent<CanvasGroup>().alpha > 0.01f && !fadeOutDone && doFadeOut)
        {
            panel.GetComponent<CanvasGroup>().alpha = 0f;
        }
        else if (sentenceToWrite.Length > charIndex && panel.GetComponent<CanvasGroup>().alpha > 0.9f)
        {
            dialogueText.text = sentenceToWrite;
            charIndex = sentenceToWrite.Length;
            typeTime = 0f;
        }
        else
            DisplayNextSentence();
    }

    private void StartDialogue()
    {
        isBusy = true;

        if (dialogues[di].playerIndex == 0)
        {
            mDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;
            lDialogueCanvas.enabled = false;

            panel = mDialogueCanvas.transform.GetChild(0);
        }
        else if (dialogues[di].playerIndex == 1)
        {
            mDialogueCanvas.enabled = false;
            lDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;

            panel = lDialogueCanvas.transform.GetChild(0);
        }
        else if (dialogues[di].playerIndex == 2)
        {
            mDialogueCanvas.enabled = false;
            rDialogueCanvas.enabled = true;
            lDialogueCanvas.enabled = false;

            panel = rDialogueCanvas.transform.GetChild(0);
        }
        else
        {
            Debug.LogError("Player Index invalid value, must be 0, 1 or 2");
            panel = mDialogueCanvas.transform.GetChild(0);
        }

        nameText = panel.GetChild(0).GetComponent<TextMeshProUGUI>();
        image = panel.GetChild(1).GetComponent<Image>();
        dialogueText = panel.GetChild(2).GetComponent<TextMeshProUGUI>();

        nameText.text = dialogues[di].npcName;
        image.sprite = dialogues[di].npcSprite;
        dialogueText.text = string.Empty;
        writtenSentence = string.Empty;
        sentenceToWrite = string.Empty;
        charIndex = 0;

        if (dialogues[di].overrideSpeed)
            actualTypeSpeed = dialogues[di].typeSpeed;
        else
            actualTypeSpeed = typeSpeed;


        freezeCamera = dialogues[di].freezeCamera;
        GameManager.instance.SetFreezeGame(dialogues[di].freezeTime);

        sentences.Clear();

        for(int i = 0; i < dialogues[di].sentences.Length; i++)
        {
            sentences.Enqueue(dialogues[di].sentences[i]);
        }

        if(dialogues[di].fadeIn)
        {
            panel.GetComponent<CanvasGroup>().alpha = 0f;
        }
        else
        {
            panel.GetComponent<CanvasGroup>().alpha = 1f;
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            BetweenNextDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        sentenceToWrite = sentence;
        writtenSentence = string.Empty;
        charIndex = 0;

        if (dialogues[di].voiceOver)
        {
            if (dialogues[di].audioClips.Length > sentenceIndex)
            {
                if (dialogues[di].audioClips[sentenceIndex] != null)
                {
                    audioSources[0].clip = dialogues[di].audioClips[sentenceIndex];
                    audioSources[0].Play();
                }
                else
                    Debug.LogWarning("No audioclip on dialogue with index " + di + ". This is not an error, the game will work just fine without it.");
            }
        }

        sentenceIndex++;
    }

    private void BetweenNextDialogue()
    {
        if (dialogues[di].fadeOut)
            doFadeOut = true;
        else
            DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        di++;
        sentenceIndex = 0;

        if (dialogues[di - 1].moveCamera)
            MoveCamera();

        if (di == dialogues.Length)
            EndDialogue();
        else if (dialogues[di - 1].waitTime > 0f)
            StartCoroutine(DialogueDelay());
        else
            StartDialogue();
    }

    private IEnumerator DialogueDelay()
    {
        mDialogueCanvas.enabled = false;
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        yield return new WaitForSeconds(dialogues[di - 1].waitTime);
        StartDialogue();
    }

    private void MoveCamera()
    {
        moveCamera = true;
        float moveCameraSpeed = dialogues[di - 1].moveCameraSpeed;
        float moveCameraX = dialogues[di - 1].moveCameraX;
        float moveCameraWait = dialogues[di - 1].moveCameraWait;

        camController.DialogueMove(moveCameraSpeed, moveCameraX, moveCameraWait);
    }

    public void EndDialogue()
    {
        mDialogueCanvas.enabled = false;
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        di = 0;
        sentenceIndex = 0;
        GameManager.instance.SetFreezeGame(false);
        freezeCamera = false;
        fadeInDone = false;

        isBusy = false;
        dialogues = null;
    }
}
