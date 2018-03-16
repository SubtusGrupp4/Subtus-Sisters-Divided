using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO: More tooltips
// TODO: Fix camera movement
// TODO: Fix freezing camera
// TODO: Fix freezing the game
// TODO: Make boxes fade in/out at the same time
// TODO: Organize methods
// TODO: Is isBusy even used?
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

    [Header("Input")]
    [SerializeField]
    private string inputString;
    [SerializeField]
    private float waitTimeAfterAllWritten = 1f;

    private TextMeshProUGUI nameText;
    private Image image;
    private TextMeshProUGUI dialogueText;

    private int di = 0; // Dialogue Index
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

    private bool player1Pressed = false;
    private bool player2Pressed = false;

    private bool startSwitch = false;

    private void Awake()
    {
        CreateSingleton();
    }

    void Start ()
    {
        if (lDialogueCanvas == null || rDialogueCanvas == null || mDialogueCanvas == null)
            Debug.LogWarning("Missing references on DialogueManager. Assign in the inspector on the DialogueManager object.");

        sentences = new Queue<string>();

        // Disable all of the canvases
        mDialogueCanvas.enabled = false;
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;

        // Two values enabled switching the speed between dialogues, but then resetting to the original value
        actualTypeSpeed = typeSpeed;

        camController = Camera.main.GetComponent<CameraController>();

        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)    // Check if there are too few audiosources to work correctly
            Debug.LogError("DialogueManager requires 2 AudioSource components to function.");
        audioSources[1].volume = typingVolume;  // Assign the typing volume
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Get the dialogue scripts from the trigger
    public void FetchDialogue(Dialogue[] dialogues)
    {
        if (isBusy)
            EndDialogue();

        this.dialogues = dialogues;     // Assign the dialogues
        foreach(Dialogue d in dialogues)
        {
            if(d.causeSwitch)
            {
                startSwitch = true;
                GameManager.instance.playerTop.GetComponent<PlayerController>().PreventInput(true);
                GameManager.instance.playerBot.GetComponent<PlayerController>().PreventInput(true);
            }
        }
        StartDialogue();                // Start processing the dialogue
    }

    private void Update()
    {
        // If there are dialogues to process
        if (dialogues != null && dialogues.Length != 0)
        {
            // playerIndex is what player is required to press a button to interact with the dialogue
            switch (dialogues[di].playerIndex)
            {
                case PlayerIndex.Both:
                    if(!player1Pressed && !player2Pressed)
                        if(Input.GetAxis(inputString + "_C1") > 0.1f || Input.GetAxis(inputString + "_C2") > 0.1f)
                            InputGet();
                    break;
                case PlayerIndex.Player1:
                    if(!player1Pressed)
                        if (Input.GetAxis(inputString + "_C1") > 0.1f)
                            InputGet();
                    break;
                case PlayerIndex.Player2:
                    if(!player2Pressed)
                        if (Input.GetAxis(inputString + "_C2") > 0.1f)
                            InputGet();
                    break;
            }
            if (sentenceToWrite.Length > charIndex)         // If there are more characters to write than are currently displayed
            {
                if (dialogues[di].fadeIn && !fadeInDone)    // Prevent characters being typed if the boxes are fading
                    return;

                StartCoroutine(TypeTimer(actualTypeSpeed / 100f));
            }
            else    // If done writing
            {
                //StartCoroutine(SkipTimer());
            }
            // If there is dialogue to write, and the panel should fade in but not currently out
            if(dialogues != null && dialogues[di].fadeIn && !doFadeOut)
            {
                if (panel.GetComponent<CanvasGroup>().alpha < 1f)   // If the panel is not fully opague
                {
                    // Add to the alpha, use it as a timer
                    panel.GetComponent<CanvasGroup>().alpha += Time.deltaTime / dialogues[di].fadeTime;
                    fadeInDone = false;
                }
                else    // If the panel is fully opague
                {
                    panel.GetComponent<CanvasGroup>().alpha = 1f;   // Prevent values larger than 1 (not sure if possible)
                    if (!fadeInDone)            // Prevent this from happening more than once
                    {
                        DisplayNextSentence();  // Grab the next sentence, and start typing it
                        fadeInDone = true;
                    }
                }
            }
            // If the panel should fade out
            if(dialogues != null && dialogues[di].fadeOut && doFadeOut)
            {
                if (panel.GetComponent<CanvasGroup>().alpha > 0f)   // If the panel is ot fully transparent
                {
                    // Remove from the alpha, use it as a timer
                    panel.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / dialogues[di].fadeTime;
                    fadeOutDone = false;
                }
                else    // If the panel is fully transpaent
                {
                    panel.GetComponent<CanvasGroup>().alpha = 0f;   // Prevent values smaller than 0 (not sure if possible)
                    if (!fadeOutDone)   // Prevent this from happening more than once
                    {
                        DisplayNextDialogue();  // Grab the next sentence, and start typing it
                        fadeOutDone = true;
                        doFadeOut = false;
                    }
                }
            }
        }

        player1Pressed = (Input.GetAxis(inputString + "_C1") > 0.1f);
        player2Pressed = (Input.GetAxis(inputString + "_C2") > 0.1f);
    }

    private IEnumerator TypeTimer(float time)
    {
        yield return new WaitForSeconds(time);

        if (charIndex < sentenceToWrite.Length)
        {
            char toWrite = sentenceToWrite[charIndex];  // Get the current character to be added to the string

            writtenSentence += toWrite;                 // Add it to the string
            dialogueText.text = writtenSentence;        // Set the text in the dialogue box to the current string
            charIndex++;                            // Increment the character index
            if (dialogues[di].typeSounds && toWrite != ' ')     // If sounds are enabled and the character is not a space
            {
                int typeSoundIndex = Random.Range(0, typingSounds.Length);  // Get a random typing sound from the array TODO: Make this not repeating the previous sound
                audioSources[1].clip = typingSounds[typeSoundIndex];        // Play the selected sound
                audioSources[1].Play();
            }
        }
    }

    private IEnumerator SkipTimer()
    {
        yield return new WaitForSeconds(waitTimeAfterAllWritten);
        if (sentenceToWrite.Length <= charIndex)
            DisplayNextSentence();
    }

    // If the button to skip is pressed, not dependant on which player, that is handled in Update()
    private void InputGet()
    {
        if (dialogues[di].fadeIn && panel.GetComponent<CanvasGroup>().alpha < 0.99f && !fadeInDone)  // If fading in
            panel.GetComponent<CanvasGroup>().alpha = 1f;                                           // Finish the fade
        else if (dialogues[di].fadeOut && panel.GetComponent<CanvasGroup>().alpha > 0.01f && !fadeOutDone && doFadeOut)  // If fading out
            panel.GetComponent<CanvasGroup>().alpha = 0f;                                                               // Finish the fade
        else if (sentenceToWrite.Length > charIndex && panel.GetComponent<CanvasGroup>().alpha > 0.9f)  // If the text is not done being written
        {
            dialogueText.text = sentenceToWrite;    // Complete the written sentence
            charIndex = sentenceToWrite.Length;     // Skip to the last character, preventing the typing to occur
            typeTime = 0f;                          // Reset the typing timer
        }
        else // If not fading and is finished typing
            DisplayNextSentence();  // Grab the next sentence, and start typing it
    }

    // Initial start of the dialogue
    // Runs for each dialogue script
    private void StartDialogue()
    {
        if (dialogues.Length == 0)
            return;

        isBusy = true;

        // Enable the correct dialogue canvas, depending on the player index
        if (dialogues[di].playerIndex == PlayerIndex.Both)
        {
            mDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;
            lDialogueCanvas.enabled = false;

            panel = mDialogueCanvas.transform.GetChild(0);
        }
        else if (dialogues[di].playerIndex == PlayerIndex.Player1)
        {
            mDialogueCanvas.enabled = false;
            lDialogueCanvas.enabled = true;
            rDialogueCanvas.enabled = false;

            panel = lDialogueCanvas.transform.GetChild(0);
        }
        else if (dialogues[di].playerIndex == PlayerIndex.Player2)
        {
            mDialogueCanvas.enabled = false;
            rDialogueCanvas.enabled = true;
            lDialogueCanvas.enabled = false;

            panel = rDialogueCanvas.transform.GetChild(0);
        }

        // Assign the correct name text, image and dialogue text boxes
        if(panel.GetChild(0).GetComponent<TextMeshProUGUI>() != null)
            nameText = panel.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(panel.GetChild(1).GetComponent<Image>() != null)
            image = panel.GetChild(1).GetComponent<Image>();
        dialogueText = panel.GetChild(2).GetComponent<TextMeshProUGUI>();

        // Display the name and image
        if(nameText != null)
            nameText.text = dialogues[di].npcName;
        if(image != null)
            image.sprite = dialogues[di].npcSprite;

        // Empty all strings, clearing them from previous values
        dialogueText.text = string.Empty;
        writtenSentence = string.Empty;
        sentenceToWrite = string.Empty;
        charIndex = 0;  // Reset the char index, resetting the typing

        // Assignt the correct typing speed for the current dialogue
        if (dialogues[di].overrideSpeed)
            actualTypeSpeed = dialogues[di].typeSpeed;
        else
            actualTypeSpeed = typeSpeed;

        // TODO: Make these work
        //freezeCamera = dialogues[di].freezeCamera;                      // Freezing camera
        //GameManager.instance.SetFreezeGame(dialogues[di].freezeTime);   // Freezing game

        // Clear the queue of sentences from previous values
        sentences.Clear();

        for(int i = 0; i < dialogues[di].sentences.Length; i++)     // For each sentence on the dialogue dialogue script
        {
            sentences.Enqueue(dialogues[di].sentences[i]);          // Add them to the sentences queue
        }

        if(dialogues[di].fadeIn)                                    // If fading in
            panel.GetComponent<CanvasGroup>().alpha = 0f;           // Make the panel invisible to allow fading in
        else
        {
            panel.GetComponent<CanvasGroup>().alpha = 1f;           // Else, make it opague
            DisplayNextSentence();                                  // and immediately display the next sentence
        }
    }

    // Fetches the next sentence
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0 && dialogues[di].manualSkip)   // If there is no more sentences to read
        {
            BetweenNextDialogue();  // Go to the "between" next dialogue
        }
        else if (sentences.Count != 0)
        {
            string sentence = sentences.Dequeue();  // Store the next sentence into a string, and remove it (dequeue)
            sentenceToWrite = sentence;
            writtenSentence = string.Empty;         // Clear what has been typed
            charIndex = 0;                          // Reset the typing char index

            if (dialogues[di].voiceOver)            // If there is voiceover to play
            {
                if (dialogues[di].audioClips.Length > sentenceIndex)    // If there are more audioclips than the current sentence index
                {
                    if (dialogues[di].audioClips[sentenceIndex] != null)    // And the current sentence has an audioclip assigned to it
                    {
                        audioSources[0].clip = dialogues[di].audioClips[sentenceIndex];
                        audioSources[0].Play();                             // Play the voiceover clip
                    }
                    else
                        Debug.LogWarning("No audioclip on dialogue with index " + di + ". This is not an error, the game will work just fine without it.");
                }
            }

            sentenceIndex++;    // Increment the sentence index for next time
        }
    }

    private void BetweenNextDialogue()
    {
        if (dialogues[di] != null && dialogues[di].fadeOut)  // Fade out, if it is supposed to
            doFadeOut = true;
        else    // Otherwise just skip to the next dialogue
            DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        if (dialogues[di].causeSwitch)
            startSwitch = true;

        di++;                   // Increment the dialogue index
        sentenceIndex = 0;      // Reset the sentence index

        if (dialogues[di - 1].moveCamera)   // If the camera is supposed to move, do so
            MoveCamera();

        if (di == dialogues.Length)     // If the last dialogue was the last one, end the dialogue
            EndDialogue();
        else if (dialogues[di - 1].waitTime > 0f)   // Otherwise, if it is supposed to wait, start next dialogue after a delay
            StartCoroutine(DialogueDelay());
        else                            // Otherwise, just start the next dialogue
            StartDialogue();
    }

    private IEnumerator DialogueDelay()
    {
        // Hide all the canvases
        mDialogueCanvas.enabled = false;
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;
        yield return new WaitForSeconds(dialogues[di - 1].waitTime);    // Wait a set time
        StartDialogue();                                                // Start the next dialogue
    }

    private void MoveCamera()
    {
        // Assign all the values
        moveCamera = true;
        float moveCameraSpeed = dialogues[di - 1].moveCameraSpeed;
        float moveCameraX = dialogues[di - 1].moveCameraX;
        float moveCameraWait = dialogues[di - 1].moveCameraWait;

        // Then send them to the camera controller
        camController.DialogueMove(moveCameraSpeed, moveCameraX, moveCameraWait);
    }

    public void EndDialogue()
    {
        if (startSwitch)
            SwitchPlayers.instance.StartSwitch();

        // Disable all of the canvases
        mDialogueCanvas.enabled = false;
        lDialogueCanvas.enabled = false;
        rDialogueCanvas.enabled = false;

        // Reset the indices
        di = 0;
        sentenceIndex = 0;

        // Unfreeze
        //GameManager.instance.SetFreezeGame(false);
        freezeCamera = false;

        // Reset more values
        fadeInDone = false;
        isBusy = false;

        // Null the dialoues
        dialogues = null;
    }
}
