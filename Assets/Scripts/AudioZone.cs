using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider))]
public class AudioZone : MonoBehaviour
{
    [System.Serializable]
    private struct Parameter
    {
        public string name;
        [Range(0f, 1f)]
        public float value;
    }

    [SerializeField]
    [Tooltip("What emitter to target and change.")]
    FMODUnity.StudioEventEmitter emitter;

    [SerializeField]
    [Tooltip("Each parameter will change the corresponding parameter in FMOD")]
    private List<Parameter> parameters;

    private List<Parameter> oldParameters;

    [SerializeField]
    [Tooltip("If true, will return all values to the initial values from when the scene started when the player exits the trigger.")]
    private bool resetOnExit = false;

    [SerializeField]
    [Tooltip("This will clear the parameters and fetch all parameters from FMOD. If it doesn't update, play and then stop the game. It should fetch the parameters. If it still doesn't, make sure that under 'Initial Paremeters' in the event emitter component, that all parameters are checked.")]
    private bool fetchParameters = true;

    private void Start()
    {
        if (resetOnExit)
        {
            for (int i = 0; i < emitter.Params.Length; i++)
            {
                Parameter p = new Parameter
                {
                    name = emitter.Params[i].Name,
                    value = emitter.Params[i].Value
                };

                oldParameters.Add(p);
            }
        }
    }

    private void Awake()
    {
        if (fetchParameters)
        {
            parameters.Clear();
            for (int i = 0; i < emitter.Params.Length; i++)
            {
                Parameter p = new Parameter
                {
                    name = emitter.Params[i].Name,
                    value = emitter.Params[i].Value
                };

                parameters.Add(p);
            }
            fetchParameters = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            foreach (Parameter p in parameters)
                emitter.SetParameter(p.name, p.value);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && resetOnExit)
            foreach (Parameter p in oldParameters)
                emitter.SetParameter(p.name, p.value);
    }
}
