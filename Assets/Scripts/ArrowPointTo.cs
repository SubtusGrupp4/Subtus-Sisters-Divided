using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointTo : MonoBehaviour
{

    private string rightXAxis = "Horizontal_Fire";
    private string rightYAxis = "Vertical_Fire";

    [SerializeField]
    private Controller controller;

    [SerializeField]
    Vector2 direction;

    private bool showArrow = false;

    private SpriteRenderer sr;

    // Use this for initialization
    void Start ()
    {
        string controllerCode;

        controllerCode = controller == Controller.Player1 ? "_C1" : "_C2";

        rightXAxis += controllerCode;
        rightYAxis += controllerCode;

        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    	float x = Input.GetAxisRaw(rightXAxis);
        float y = Input.GetAxisRaw(rightYAxis);

        if (y <= 0f)
			x = -x;

        direction = new Vector2(x, y);
        Quaternion rotation = Quaternion.LookRotation(direction, Vector2.right);
        transform.rotation = new Quaternion(0f, 0f, rotation.x, rotation.w);
        
    }

    public void ShowArrow(bool show)
    {
        sr.enabled = show;
        showArrow = show;
    }
}
