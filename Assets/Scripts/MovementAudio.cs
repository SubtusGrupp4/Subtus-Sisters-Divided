using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
    Grass, Gravel, Wood,
    GrassGross
}

public class MovementAudio : MonoBehaviour
{
    public GroundType groundType = GroundType.Grass;

    [FMODUnity.EventRef]
    [SerializeField]
    private string grassEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string grassGrossEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string concreteEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string gravelEvent;

    [FMODUnity.EventRef]
    [SerializeField]
    private string woodEvent;

    public void Footstep()
    {
        string eventPath = CheckGroundType();
        FMODUnity.RuntimeManager.PlayOneShot(eventPath, transform.position);
    }

    public void Landing()
    {
        string eventPath = CheckGroundType();
        eventPath += "Landing";
        FMODUnity.RuntimeManager.PlayOneShot(eventPath, transform.position);
    }

    public void Jump()
    {
        string eventPath = CheckGroundType();
        eventPath += "Jump";
        FMODUnity.RuntimeManager.PlayOneShot(eventPath, transform.position);
    }

    private string CheckGroundType()
    {
        string eventPath;

        switch (groundType)
        {
            case GroundType.Gravel:
                eventPath = gravelEvent;
                break;
            case GroundType.Wood:
                eventPath = woodEvent;
                break;
            case GroundType.GrassGross:
                eventPath = grassGrossEvent;
                break;
            default:
                eventPath = grassEvent;
                break;
        }

        return eventPath;
    }

    public void SetGroundType(int layerID)
    {
        switch (layerID)
        {
            case 9:
                groundType = GroundType.Gravel;
                break;
            case 10:
                groundType = GroundType.Wood;
                break;
            case 11:
                groundType = GroundType.GrassGross;
                break;
            default:
                groundType = GroundType.Grass;
                break;
        }
    }
}
