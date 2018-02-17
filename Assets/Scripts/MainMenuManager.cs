using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MainMenuManager : MonoBehaviour {

	[SerializeField]
	private GameObject lightUI;
	[SerializeField]
	private GameObject darkUI;

	[SerializeField]
	private GameObject light;
	[SerializeField]
	private GameObject dark;

	[SerializeField]
	private GameObject pressAnyKey;

	private bool useDark = false;
	private bool isActivated = false;
	private bool isDonePlaying = false;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(KeyTimer());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey && !isActivated) 
		{
			lightUI.SetActive(true);
			lightUI.GetComponentInChildren<PlayableDirector>().Play();
			StartCoroutine(DonePlaying((float)lightUI.transform.GetChild(0).GetComponent<PlayableDirector>().duration));
			pressAnyKey.SetActive(false);
			isActivated = true;
		}

		for(int i = 0; i < lightUI.transform.childCount; i++) 
		{
			darkUI.transform.GetChild(i).position = lightUI.transform.GetChild(i).position;
		}

		if(isDonePlaying) 
		{
			if(isActivated && useDark) 
			{
				dark.SetActive(true);
				light.SetActive(false);
			}
			else if(isActivated) 
			{
				dark.SetActive(false);
				light.SetActive(true);
			}

			if(Input.GetKeyDown(KeyCode.P) && isActivated)
				useDark = !useDark;
			}
	}

	IEnumerator KeyTimer() 
	{
		yield return new WaitForSeconds(5f);
		if(!isActivated)
			pressAnyKey.SetActive(true);
			
	}

	IEnumerator DonePlaying(float animationLength) 
	{
		yield return new WaitForSeconds(animationLength);
		isDonePlaying = true;
	}
}
