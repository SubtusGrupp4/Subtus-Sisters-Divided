using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuzzerTarget))]
public class FlyReactToPlayer : MonoBehaviour {

    [SerializeField]
    private ParticleSystem[] flies;
    private Vector2 startPosition;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector2.Distance(GameManager.instance.playerBot.position, transform.position) < GetComponent<BuzzerTarget>().radius * 1.5f)
        {
            foreach(ParticleSystem fly in flies)
            {
                var emission = fly.emission;
                emission.rateOverTime = 15f;

                var main = fly.main;
                main.startSpeed = 2f;
            }
            transform.position = startPosition + (Random.insideUnitCircle / 35f);
        }
        else
        {
            foreach (ParticleSystem fly in flies)
            {
                var emission = fly.emission;
                emission.rateOverTime = 1f;

                var main = fly.main;
                main.startSpeed = 1f;
            }
            transform.position = startPosition;
        }
	}
}
