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
    public bool isBusy = false;
    public bool moveCamera = false;

    [Header("Right Dialogue")]
    [Tooltip("The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
    [SerializeField]
    private Canvas rDialogueCanvas;

    [Header("Left Dialogue")]
    [Tooltip("The children of this object will be fetched at runtime. Do not change the order of the child objects!")]
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
    [SerializeField]
    private float scrollSpeed = 1f;
    private float actualScrollSpeed;

    private CameraController camController;

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

    private void FixedUpdate()
    {
        if(sentenceToWrite.Length > charIndex)
        {
            writtenSentence += sentenceToWrite[charIndex];
            dialogueText.text = writtenSentence;
            charIndex++;
        }
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
        if(Input.GetKeyDown(KeyCode.E))
        {
            DisplayNextSentence();
        }
    }

    public void FetchDialogue(Dialogue[] dialogues)
    {
        this.dialogues = dialogues;

        StartDialogue();
    }

    private void StartDialogue()
    {
        isBusy = true;

        if(dialogues[dialogueIndex].rightAligned)
        {
            rDialogueCanvas.enabled = true;
            lDialogueCanvas.enabled = false;

            Transform rPanel = rDialogueCanvas.transform.GetChild(0);

            nameText = rPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
            image = rPanel.GetChild(1).GetComponent<Image>();
            dialogueText = rPanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            lDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;

            Transform lPanel = lDialogueCanvas.transform.GetChild(0);

            nameText = lPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
            image = lPanel.GetChild(1).GetComponent<Image>();
            dialogueText = lPanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        }

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
                //Debug.Log("Sentence index: " + sentenceIndex);
                audioSource.clip = dialogues[dialogueIndex].audioClips[sentenceIndex];
                audioSource.Play();
            }
            else
                Debug.LogWarning("No audioclip on dialogue with index " + dialogueIndex);
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
