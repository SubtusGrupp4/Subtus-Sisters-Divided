using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
	public void SetAmount(float amount) 
	{
            GetComponent<Image>().fillAmount =  amount;
	}

    /*
    private SpriteRenderer sr;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	public void SetAmount(float amount) 
	{
        sr.material.SetFloat("_Cutoff", amount);
	}
    */
}
