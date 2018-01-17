using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private Queue<string> sentences;

    [SerializeField]
    private Canvas dialogueCanvas;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text dialogueText;

    private void Awake()
    {
        CreateSingleton();
    }

    void Start ()
    {
        sentences = new Queue<string>();
        dialogueCanvas.enabled = false;
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

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueCanvas.enabled = true;
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    private void EndDialogue()
    {
        dialogueCanvas.enabled = false;
    }
}
