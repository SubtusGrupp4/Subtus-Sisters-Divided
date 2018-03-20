using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    [SerializeField]
    private float interval = 0.5f;
    [SerializeField]
    private string[] variations;
    private TextMeshProUGUI text;

    private int index = 0;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = variations[index];
        StartCoroutine(LoadingDots());
    }

    private IEnumerator LoadingDots()
    {
        yield return new WaitForSeconds(interval);
        if (index < variations.Length - 1)
            index++;
        else
            index = 0;

        text.text = variations[index];
        StartCoroutine(LoadingDots());
    }
}
