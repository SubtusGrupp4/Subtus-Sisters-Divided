using UnityEngine;
using System;
using System.Collections.Generic;

public class FMODEmitter : MonoBehaviour
{
    [Header("Custom FMOD Emitter")]
    [FMODUnity.EventRef]
    public string Event = "";

    [Header("3D Settings")]
    public bool is3D = true;
    public bool OverrideAttenuation = true;
    public float OverrideMinDistance = 1.0f;
    public float OverrideMaxDistance = 20.0f;

    [Header("Parameters")]
    public FMODUnity.ParamRef[] initialParameters = new FMODUnity.ParamRef[0];

    private FMOD.Studio.EventInstance instance;

    public void SetEvent(string Event)
    {
        Debug.Log("Changed FMOD Emitter event to:" + Event);
        this.Event = Event;
    }

    public void Play()
    {
        Debug.Log("Play FMOD Emitter. Event: " + Event);
        instance = FMODUnity.RuntimeManager.CreateInstance(Event);

        // Only want to update if we need to set 3D attributes
        if (is3D)
        {
            Rigidbody2D rigidBody2D = GetComponent<Rigidbody2D>();
            Transform transform = GetComponent<Transform>();
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
        }

        if (is3D && OverrideAttenuation)
        {
            instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
            instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
        }

        instance.start();
    }

    public void Stop()
    {
        Debug.Log("Stop FMOD Emitter. Event: " + Event);
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    public void Kill()
    {
        Debug.Log("Kill FMOD Emitter. Event: " + Event);
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }

    public void SetParameter(string name, float value)
    {
        instance.setParameterValue(name, value);
    }

    public bool IsPlaying()
    {
        if (instance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            instance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
        return false;
    }
}
