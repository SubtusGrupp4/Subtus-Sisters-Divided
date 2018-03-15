using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    Button, Lever
}
public class BaseButton : MonoBehaviour
{
    [HideInInspector]
    public bool IsActive = false;

    public InteractionType Type;

    private int toggleValue = 1;

    [GiveTag]
    public string[] InteractWith = new string[] { };

    [SerializeField]
    private bool stayDown; // if triggered, stay down.
    private bool isDown;

    [Header("Sprites")]
    public bool HideOnInteraction; // Hide the sprite if the button is active.
    public Sprite stateOne;
    public Sprite stateTwo;

    [Header("Item requirments")]

    [SerializeField]
    private bool useItemIndex;
    public int itemIndex;

    [Header("Trigger behavior")]

    [SerializeField]
    private bool useTriggerIndex;
    public int triggerIndex; // Other buttons ID

    [SerializeField]
    private int triggerCount; // Amount of buttons that need to be down before you do your shit. ?? Only show if TriggerIndex is used?
    [HideInInspector]
    public int triggerCounter;

    [Header("Sounds")]
    [SerializeField]
    private bool Sounds;
    [SerializeField]
    [FMODUnity.EventRef]
    private string activationEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    private string deActivationEvent;

    private AudioSource myAudio;
    private SpriteRenderer sr;

    private List<BaseButton> otherButtons = new List<BaseButton>();
    private List<GameObject> brothers = new List<GameObject>();



   protected virtual void  Start()
    {
        if (Sounds)
            myAudio = GetComponent<AudioSource>();

        sr = GetComponent<SpriteRenderer>();

        // Find other buttons with same TriggerIndex
        otherButtons.AddRange(FindObjectsOfType<BaseButton>());

        if (useTriggerIndex)
        {
            for (int i = 0; i < otherButtons.Count; i++)
            {
                if (!otherButtons[i].GetComponent<BaseButton>().useTriggerIndex)
                    continue;

                if (otherButtons[i].GetComponent<BaseButton>().triggerIndex == triggerIndex)
                {
                    brothers.Add(otherButtons[i].gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (Type == InteractionType.Button)
            ValidityCheck(obj.gameObject, +1, true);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (Type == InteractionType.Button)
            ValidityCheck(obj.gameObject, -1, false);
    }

    private void ValidityCheck(GameObject obj, int change, bool hide)
    {
        for (int i = 0; i < InteractWith.Length; i++)
            if (obj.tag == InteractWith[i])
            {
                if (useItemIndex)
                {
                    if (obj.GetComponent<ItemIndex>()) // Incase the object dosen't have the script "ItemIndex" on it we dont want error message spam.
                    {
                        if (obj.GetComponent<ItemIndex>().Index == itemIndex)
                        {
                            ChangeCount(change);

                            if (obj.GetComponent<ItemIndex>().DestroyOnUse)
                                Destroy(obj);
                        }
                    }
                }
                else if (!useItemIndex)
                    ChangeCount(change);

                /*
                if (hide)
                    Hide();// Change to 2 pictures??, aka picture 1 and picture 2? good incase we want to Toggle a lever or something
                else
                    Show();
                    */
            }
    }

    private void ChangeCount(int change)
    {
        if (useTriggerIndex)
            for (int i = 0; i < brothers.Count; i++)
            {
                brothers[i].GetComponent<BaseButton>().triggerCounter += change;
                brothers[i].GetComponent<BaseButton>().CountChange();

                if (change > 0)
                    Hide();// Change to 2 pictures??, aka picture 1 and picture 2? good incase we want to Toggle a lever or something
               
                    

            }
        else
        {
            if (change > 0)
                Hide();// Change to 2 pictures??, aka picture 1 and picture 2? good incase we want to Toggle a lever or something
            

            triggerCounter += change;
            CountChange();
        }

        
    }

    private void Hide()
    {
        // Change to Show Picture 2
        sr.sprite = stateTwo;

        if (HideOnInteraction)
            GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Show()
    {
        sr.sprite = stateOne;

        // Change to Show picture 1
        if (HideOnInteraction)
            GetComponent<SpriteRenderer>().enabled = true;
    }

    public void CountChange()
    {
        if (!isDown)
        {
            // Activate interaction
            if (triggerCounter >= triggerCount && !IsActive)
            {
                DoStuff();
                IsActive = true;

                if (Sounds)
                    FMODUnity.RuntimeManager.PlayOneShot(activationEvent, transform.position);

                if (stayDown)
                    isDown = true;
            }
            // Deactivate interaction
            else if (triggerCounter < triggerCount && IsActive)
            {
                UndoStuff();
                IsActive = false;

                Show();

                if (Sounds)
                    FMODUnity.RuntimeManager.PlayOneShot(deActivationEvent, transform.position);

            }
        }
    }

    public void Toggle()
    {


        ChangeCount(toggleValue);
        toggleValue *= -1;

        // PLAY SOUND ?
        // CHANGE SPRITE ??
    }

    protected virtual void DoStuff()
    {

    }
    protected virtual void UndoStuff()
    {

    }
}
