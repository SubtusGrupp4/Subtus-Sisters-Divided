using UnityEngine;
using System;
using System.Collections.Generic;

public class FMODEmitter : MonoBehaviour
{
    [Header("Custom FMOD Emitter")]
    [FMODUnity.EventRef]
    public string Event = "";
    public bool TriggerOnce = false;
    public bool AllowFadeout = true;
    public FMODUnity.ParamRef[] startingParameters = new FMODUnity.ParamRef[0];

    private FMOD.Studio.EventDescription eventDescription;
    public FMOD.Studio.EventDescription EventDescription { get { return eventDescription; } }

    private FMOD.Studio.EventInstance instance;
    public FMOD.Studio.EventInstance EventInstance { get { return instance; } }

    private bool hasTriggered = false;
    private bool isQuitting = false;

    void Awake()
    {
        FMODUnity.RuntimeUtils.EnforceLibraryOrder();
        if (Event != "")
            Play();
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (!isQuitting)
            if (instance.isValid())
                FMODUnity.RuntimeManager.DetachInstanceFromGameObject(instance);
    }

    void Lookup()
    {
        eventDescription = FMODUnity.RuntimeManager.GetEventDescription(Event);
    }

    public void SetEvent(string Event)
    {
        this.Event = Event;
    }

    public void Play()
    {
        Debug.Log("Play FMOD Emitter");
        if (TriggerOnce && hasTriggered)
            return;

        if (String.IsNullOrEmpty(Event))
            return;

        if (!eventDescription.isValid())
            Lookup();

        bool isOneshot = false;
        if (!Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
            eventDescription.isOneshot(out isOneshot);
        bool is3D;
        eventDescription.is3D(out is3D);

        if (!instance.isValid())
            instance.clearHandle();

        // Let previous oneshot instances play out
        if (isOneshot && instance.isValid())
        {
            instance.release();
            instance.clearHandle();
        }

        if (!instance.isValid())
        {
            eventDescription.createInstance(out instance);

            // Only want to update if we need to set 3D attributes
            if (is3D)
            {
                var rigidBody = GetComponent<Rigidbody>();
                var rigidBody2D = GetComponent<Rigidbody2D>();
                var transform = GetComponent<Transform>();
                if (rigidBody)
                {
                    instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                    FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                }
                else
                {
                    instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                    FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                }
            }
        }

        foreach (var param in startingParameters)
            instance.setParameterValue(param.Name, param.Value);

        instance.start();

        hasTriggered = true;

    }

    public void Stop()
    {
        Debug.Log("Stop FMOD Emitter");
        if (instance.isValid())
        {
            instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
            instance.clearHandle();
        }
    }

    public void SetParameter(string name, float value)
    {
        if (instance.isValid())
            instance.setParameterValue(name, value);
    }

    public bool IsPlaying()
    {
        if (instance.isValid() && instance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            instance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
        return false;
    }
}
