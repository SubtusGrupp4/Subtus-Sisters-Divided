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
    FMODUnity.StudioEventEmitter emitter;

    [SerializeField]
    private List<Parameter> parameters;

    private List<Parameter> oldParameters;

    [SerializeField]
    private bool resetOnExit = false;
    [SerializeField]
    private bool fetchParameters = true;

    private void Start()
    {
        if (resetOnExit) {
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
        {
            foreach (Parameter p in oldParameters)
                emitter.SetParameter(p.name, p.value);
        }
    }
}
