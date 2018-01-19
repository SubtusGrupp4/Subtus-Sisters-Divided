using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public Dialogue[] dialogues;

    private void Start()
    {
        dialogues = GetComponents<Dialogue>();
    }

    public void TriggerDialogue()
    {
        DialogueManager.instance.FetchDialogue(dialogues);
    }
}
