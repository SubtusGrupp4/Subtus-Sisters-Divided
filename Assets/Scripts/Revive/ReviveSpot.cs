using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSpot : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem stars;
    [SerializeField]
    private ParticleSystem lines;
    [SerializeField]
    private SpriteRenderer buttonIcon;

    private void Start()
    {
        if(transform.position.y < 0f)
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.x);

            var main = stars.main;
            var constantMin = main.startSpeed.constantMin;
            var constantMax = main.startSpeed.constantMax;
            constantMin = -main.startSpeed.constantMin;
            constantMax = -main.startSpeed.constantMax;
        }
    }

    public void SetParticleEmission(float amount)
    {
        var emission = lines.emission;
        emission.rateOverTime = amount;
        buttonIcon.enabled = (amount < 0.1f) ? false : true;
    }
}
