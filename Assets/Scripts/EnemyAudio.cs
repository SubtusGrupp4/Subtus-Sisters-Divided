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

    void Start ()
    {
        emitter = GetComponent<FMODEmitter>();
        emitter.Play();
	}

    public void Idle()
    {
        if(idleEvent != "")
        {
            emitter.Stop();
            emitter.SetEvent(idleEvent);
            emitter.Play();
            Debug.Log(transform.name + " idle");
        }
    }

    public void Footstep()
    {
        if(walkingEvent != "")
        {
            emitter.Stop();
            emitter.SetEvent(walkingEvent);
            emitter.Play();
            Debug.Log(transform.name + " footstep");
        }
    }

    public void Attack()
    {
        if(attackEvent != "")
        {
            FMODUnity.RuntimeManager.PlayOneShot(attackEvent, transform.position);
            Debug.Log(transform.name + " attack");
        }
    }

    public void Falling()
    {
        if(fallingEvent != "")
        {
            emitter.Stop();
            emitter.SetEvent(fallingEvent);
            emitter.Play();
            Debug.Log(transform.name + " falling");
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
