using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour {

	private List<GameObject> ignoreObjects;

    [GiveTag]
    [SerializeField]
    private string[] tagsToIgnore = new string[] { };

    // Use this for initialization
    void Start ()
    {
        foreach(string tag in tagsToIgnore)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach(GameObject obj in objects)
                ignoreObjects.Add(obj);
        }
		
		foreach (GameObject obj in ignoreObjects)
			Physics2D.IgnoreCollision (obj.GetComponent<Collider2D> (), this.GetComponent<Collider2D>(), true);
	}
}
