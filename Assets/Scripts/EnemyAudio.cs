using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [SerializeField]
    private FMODEmitter[] emitters;

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
        emitters = GetComponents<FMODEmitter>();
        Loop();
	}

    public void Idle()
    {
        if(idleEvent != "")
        {
            if (emitters[0].Event != loopEvent)
            {
                emitters[0].Stop();
                emitters[0].SetEvent(idleEvent);
                emitters[0].Play();

                if (emitters[1] != null)
                {
                    emitters[1].Stop();
                    float time = emitters[0].GetLength(idleEvent);
                    StartCoroutine(LoopTimer(time));
                }
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
            emitters[0].Stop();
            emitters[0].SetEvent(attackEvent);
            emitters[0].Play();

            float time = emitters[0].GetLength(attackEvent);
            StartCoroutine(LoopTimer(time));
        }
    }

    public void Falling()
    {
        if(fallingEvent != "")
        {
            emitters[0].Stop();
            emitters[0].SetEvent(fallingEvent);
            emitters[0].Play();

            if (emitters[1] != null)
            {
                float time = emitters[0].GetLength(fallingEvent);
                StartCoroutine(LoopTimer(time));
            }
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
        if (loopEvent != "")
        {
            if (emitters[1] != null)
            {
                emitters[1].Stop();
                emitters[1].SetEvent(loopEvent);
                emitters[1].Play();
            }
        }
    }

    private IEnumerator LoopTimer(float time)
    {
        if(emitters[1] != null)
            emitters[1].Stop();

        yield return new WaitForSeconds(time);
        Loop();
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
