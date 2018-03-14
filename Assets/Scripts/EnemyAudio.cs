using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    private FMODEmitter emitter;

    [FMODUnity.EventRef]
    [SerializeField]
    private string idleEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string walkingEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string attackEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string fallingEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string growlEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string loopEvent;

    void Start ()
    {
        emitter = GetComponent<FMODEmitter>();
        emitter.Play();
	}

    public void Idle()
    {
        if(idleEvent != "")
        {
            if (emitter.Event != idleEvent)
            {
                emitter.Stop();
                emitter.SetEvent(idleEvent);
                emitter.Play();
            }
        }
    }

    public void Footstep()
    {
        if(walkingEvent != "")
        {
            FMODUnity.RuntimeManager.PlayOneShot(walkingEvent, transform.position);
        }
    }

    public void Attack()
    {
        if(attackEvent != "")
        {
            emitter.Stop();
            FMODUnity.RuntimeManager.PlayOneShot(attackEvent, transform.position);
        }
    }

    public void Falling()
    {
        if(fallingEvent != "")
        {
            FMODUnity.RuntimeManager.PlayOneShot(fallingEvent, transform.position);
        }
    }

    public void Growl()
    {
        if (fallingEvent != "")
        {
            FMODUnity.RuntimeManager.PlayOneShot(growlEvent, transform.position);
        }
    }

    public void Loop()
    {
        if(loopEvent != "")
        {
            if (emitter.Event != loopEvent)
            {
                emitter.Stop();
                emitter.SetEvent(loopEvent);
                emitter.Play();
            }
        }
    }

    /*
    public void ChangeState(EnemyAudioState state)
    {
        string eventPath;

        switch(state)
        {
            case EnemyAudioState.Attack:
                eventPath = attackEvent;
                break;
            case EnemyAudioState.Falling:
                eventPath = fallingEvent;
                break;
            case EnemyAudioState.Idle:
                eventPath = idleEvent;
                break;
            default:
                eventPath = walkingEvent;
                break;
        }

        if(eventPath == "")
        {
            emitter.Stop();
            return;
        }

        if (audioState != state)
        {
            Debug.Log("Playing " + eventPath);
            audioState = state;
            emitter.Stop();
            emitter.SetEvent(eventPath);
            emitter.Play();
        }
    }
    */
}
