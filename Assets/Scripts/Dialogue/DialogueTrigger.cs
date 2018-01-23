using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    [SerializeField]
    [Tooltip("Prevent the script from overriding the tag with \"DialogueTrigger\"")]
    private bool customTag = false;
    [HideInInspector]
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
