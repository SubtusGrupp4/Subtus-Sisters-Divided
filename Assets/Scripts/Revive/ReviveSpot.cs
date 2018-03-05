using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSpot : MonoBehaviour
{
    [SerializeField]
    private float bounceAmount = 1f;
    [SerializeField]
    private float bounceInterval = 3f;
    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = new Vector2(transform.position.x, startPos.y + (Mathf.Sin(Time.time * bounceInterval) * bounceAmount));
    }
}
