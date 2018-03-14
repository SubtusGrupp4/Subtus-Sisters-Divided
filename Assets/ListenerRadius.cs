using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerRadius : MonoBehaviour
{
    [SerializeField]
    private Color color;
    [SerializeField]
    private float min = 1f;
    [SerializeField]
    private float max = 20f;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, min);
        Gizmos.DrawWireSphere(transform.position, max);
    }
}
