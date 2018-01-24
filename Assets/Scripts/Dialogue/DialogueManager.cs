using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private AudioSource audioSource;
    private Queue<string> sentences;

    [HideInInspector]
    public bool isBusy = false;
    [HideInInspector]
    public bool moveCamera = false;

    [Tooltip("Right Dialogue Canvas. The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
    [SerializeField]
    private Canvas rDialogueCanvas;

    [Tooltip("Left Dialogue Canvas. The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
    [SerializeField]
    private Canvas lDialogueCanvas;

    private TextMeshProUGUI nameText;
    private Image image;
    private TextMeshProUGUI dialogueText;

    private int dialogueIndex = 0;
    private int sentenceIndex = 0;
    private Dialogue[] dialogues;

    private int charIndex = 0;
    private string sentenceToWrite = string.Empty;
    private string writtenSentence = string.Empty;

    [Space]
    [Tooltip("The global speed in hundreds of a second for each character to appear. Can be overrided on individual Dialogue components.")]
    [SerializeField]
    private float scrollSpeed = 8f;
    private float actualScrollSpeed;
    private float scrollTime = 0f;

    private CameraController camController;

    [HideInInspector]
    public bool freezeCamera = false;

    private void Awake()
    {
        CreateSingleton();
    }

    void Start ()
    {
        if (lDialogueCanvas == null || rDialogueCanvas == null)
            Debug.LogWarning("Missing references on DialogueManager. Assign in the inspector on the DialogueManager object.");


        sentences = new Queue<string>();
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        audioSource = GetComponent<AudioSource>();
        actualScrollSpeed = scrollSpeed;

        camController = Camera.main.GetComponent<CameraController>();
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (dialogues != null)
        {
            switch (dialogues[dialogueIndex].playerIndex )
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
                if (scrollTime < 1f)
                {
                    scrollTime += Time.deltaTime / (actualScrollSpeed / 100f);
                }
                else
                {
                    writtenSentence += sentenceToWrite[charIndex];
                    dialogueText.text = writtenSentence;
                    charIndex++;
                    scrollTime = 0f;
                }
            }
        }
    }

    private void InputGet()
    {
        if (sentenceToWrite.Length > charIndex)
        {
            dialogueText.text = sentenceToWrite;
            charIndex = sentenceToWrite.Length;
            scrollTime = 0f;
        }
        else
            DisplayNextSentence();
    }

    public void FetchDialogue(Dialogue[] dialogues)
    {
        this.dialogues = dialogues;

        StartDialogue();
    }

    private void StartDialogue()
    {
        isBusy = true;

        Transform panel;

        if(dialogues[dialogueIndex].rightAligned)
        {
            rDialogueCanvas.enabled = true;
            lDialogueCanvas.enabled = false;

            panel = rDialogueCanvas.transform.GetChild(0);
        }
        else
        {
            lDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;

            panel = lDialogueCanvas.transform.GetChild(0);
        }

        nameText = panel.GetChild(0).GetComponent<TextMeshProUGUI>();
        image = panel.GetChild(1).GetComponent<Image>();
        dialogueText = panel.GetChild(2).GetComponent<TextMeshProUGUI>();

        nameText.text = dialogues[dialogueIndex].npcName;
        image.sprite = dialogues[dialogueIndex].npcSprite;
        dialogueText.text = string.Empty;
        writtenSentence = string.Empty;
        sentenceToWrite = string.Empty;
        charIndex = 0;

        if (dialogues[dialogueIndex].overrideSpeed)
            actualScrollSpeed = dialogues[dialogueIndex].scrollSpeed;
        else
            actualScrollSpeed = scrollSpeed;


        freezeCamera = dialogues[dialogueIndex].freezeCamera;
        GameManager.instance.SetFreezeGame(dialogues[dialogueIndex].freezeTime);

        sentences.Clear();

        for(int i = 0; i < dialogues[dialogueIndex].sentences.Length; i++)
        {
            sentences.Enqueue(dialogues[dialogueIndex].sentences[i]);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            DisplayNextDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        sentenceToWrite = sentence;
        writtenSentence = string.Empty;
        charIndex = 0;

        if (dialogues[dialogueIndex].audioClips.Length > sentenceIndex)
        {
            if (dialogues[dialogueIndex].audioClips[sentenceIndex] != null)
            {
                audioSource.clip = dialogues[dialogueIndex].audioClips[sentenceIndex];
                audioSource.Play();
            }
            else
                Debug.LogWarning("No audioclip on dialogue with index " + dialogueIndex + ". This is not an error, the game will work just fine without it.");
        }

        sentenceIndex++;
    }

    private void DisplayNextDialogue()
    {
        dialogueIndex++;
        sentenceIndex = 0;

        if (dialogues[dialogueIndex - 1].moveCamera)
            MoveCamera();

        if (dialogueIndex == dialogues.Length)
            EndDialogue();
        else if (dialogues[dialogueIndex - 1].waitTime > 0f)
            StartCoroutine(DialogueDelay());
        else
            StartDialogue();
    }

    private IEnumerator DialogueDelay()
    {
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        yield return new WaitForSeconds(dialogues[dialogueIndex - 1].waitTime);
        StartDialogue();
    }

    private void MoveCamera()
    {
        moveCamera = true;
        float moveCameraSpeed = dialogues[dialogueIndex - 1].moveCameraSpeed;
        float moveCameraX = dialogues[dialogueIndex - 1].moveCameraX;
        float moveCameraWait = dialogues[dialogueIndex - 1].moveCameraWait;

        camController.DialogueMove(moveCameraSpeed, moveCameraX, moveCameraWait);
    }

    public void EndDialogue()
    {
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        dialogueIndex = 0;
        sentenceIndex = 0;
        GameManager.instance.SetFreezeGame(false);
        freezeCamera = false;

        isBusy = false;
    }
}
