using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour {

	private List<GameObject> ignoreObjects = new List<GameObject>();

    [GiveTag]
    [SerializeField]
    private string[] tagsToIgnore = new string[] { };

    // Use this for initialization
    void Start ()
    {
        foreach (string tag in tagsToIgnore)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
                    ignoreObjects.Add(obj);
        }
        if(ignoreObjects.Count != 0)
            foreach (GameObject obj in ignoreObjects)
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
	}
}
