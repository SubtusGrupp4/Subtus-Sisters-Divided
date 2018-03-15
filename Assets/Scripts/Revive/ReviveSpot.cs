using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveSpot : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem stars;
    [SerializeField]
    private ParticleSystem lines;
    [SerializeField]
    private Image radialProgressBar; 

    private float timer;
    private float timerAmount = 1f;
    private bool doTimer = false;

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

        timer = SafepointManager.instance.pickupTime;
        StartCoroutine(SafepointManager.instance.PickupTimer());
    }

    public void Update()
    {
        timerAmount -= Time.deltaTime / timer;
        radialProgressBar.fillAmount = timerAmount;
    }
    public void SetParticleEmission(float amount)
    {
        var emission = lines.emission;
        emission.rateOverTime = amount;
    }

    public void DestroySelf()
    {
        GetComponent<DisplayIconTrigger>().DestroyIcon();
        Destroy(gameObject);
    }
}
