using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    [SerializeField]
    private bool customTag = false;
    public Dialogue[] dialogues;

    private void Start()
    {
        if(!customTag)
            transform.tag = "DialogueTrigger";

        dialogues = GetComponents<Dialogue>();
    }

    public void TriggerDialogue()
    {
        DialogueManager.instance.FetchDialogue(dialogues);
    }
}
