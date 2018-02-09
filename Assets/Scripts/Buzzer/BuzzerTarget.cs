using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzerTarget : MonoBehaviour {

    public float radius;    // The buzzer will try to stay within this radius

    // Visually show in the Scene view where the target is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
