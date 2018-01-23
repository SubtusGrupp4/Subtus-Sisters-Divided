using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private AudioSource audioSource;
    private Queue<string> sentences;
    public bool isBusy = false;
    public bool moveCamera = false;

    [SerializeField]
    private Canvas dialogueCanvas;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text dialogueText;

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
        sentences = new Queue<string>();
        dialogueCanvas.enabled = false;
        audioSource = GetComponent<AudioSource>();

        camController = Camera.main.GetComponent<CameraController>();

        if (dialogueCanvas == null || nameText == null || image == null || dialogueText == null)
            Debug.LogWarning("Missing references on DialogueManager. Assign in the inspector.");
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

        dialogueCanvas.enabled = true;
        nameText.text = dialogues[dialogueIndex].npcName;
        image.sprite = dialogues[dialogueIndex].npcSprite;

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
        dialogueText.text = sentence;

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
        dialogueCanvas.enabled = false;
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
        dialogueCanvas.enabled = false;
        dialogueIndex = 0;
        sentenceIndex = 0;
        GameManager.instance.SetFreezeGame(false);
        freezeCamera = false;

        isBusy = false;
    }
}
