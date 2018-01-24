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
    [SerializeField]
    private Canvas rDialogueCanvas;
    private TextMeshProUGUI rNameText;
    private Image rImage;
    private TextMeshProUGUI rDialogueText;

    [Header("Left Dialogue")]
    [SerializeField]
    private Canvas lDialogueCanvas;
    private TextMeshProUGUI lNameText;
    private Image lImage;
    private TextMeshProUGUI lDialogueText;

    private int dialogueIndex = 0;
    private int sentenceIndex = 0;
    private Dialogue[] dialogues;

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
        else
        {
            Transform rPanel = rDialogueCanvas.transform.GetChild(0);
            Transform lPanel = lDialogueCanvas.transform.GetChild(0);

            rNameText = rPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
            rImage = rPanel.GetChild(1).GetComponent<Image>();
            rDialogueText = rPanel.GetChild(2).GetComponent<TextMeshProUGUI>();

            lNameText = lPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
            lImage = lPanel.GetChild(1).GetComponent<Image>();
            lDialogueText = lPanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        }

        sentences = new Queue<string>();
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        audioSource = GetComponent<AudioSource>();

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

            rNameText.text = dialogues[dialogueIndex].npcName;
            rImage.sprite = dialogues[dialogueIndex].npcSprite;
        }
        else
        {
            lDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;
            lNameText.text = dialogues[dialogueIndex].npcName;
            lImage.sprite = dialogues[dialogueIndex].npcSprite;
        }

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
        if(dialogues[dialogueIndex].rightAligned)
            rDialogueText.text = sentence;
        else
        {
            lDialogueText.text = sentence;
        }

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
