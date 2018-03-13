using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAudioState
{
    Idle, Walking, Attack, Falling
}

public class EnemyAudio : MonoBehaviour
{
    [SerializeField]
    private EnemyAudioState audioState;

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
	}

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
            audioState = state;
            emitter.Stop();
            emitter.SetEvent(eventPath);
            emitter.Play();
        }
    }
}
