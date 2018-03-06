using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAction : MonoBehaviour
{
    private string inputString = "Interacting";

    [GiveTag]
    [SerializeField]
    private string[] pickUp;

    [GiveTag]
    [SerializeField]
    private string[] lever;

    [GiveTag]
    [SerializeField]
    private string[] walls;

    [SerializeField]
    private float range;

    private bool isCorrectObj;
    private bool isKey;

    private bool isWielding;
    private bool axisInUse = false;

    PlayerController playerCont;
    Pickup pickUpShort;

    // Use this for initialization
    void Awake()
    {
        playerCont = GetComponent<PlayerController>();
        pickUpShort = GetComponent<Pickup>();

        //  inputString += playerCont.controllerCode;

        if (playerCont.player == Controller.Player1)
        {
            inputString += "_C1";
        }
        else
        {
            inputString += "_C2";
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get Ipnut
        if (Input.GetAxisRaw(inputString) != 0 && !axisInUse)
        {           
            axisInUse = true;
            // If we have the key we want to drop it
            if (isWielding)
            {
                pickUpShort.DropIt();
                isWielding = false;
            }
            // Use action code to find out if a object is blocked or not
            else if (Actions(pickUp) != null)
            {
                // Execute what to do when it's not blocked
                GameObject obj = Actions(pickUp);
                pickUpShort.PickItUp(obj.gameObject);            
                isWielding = true;
            }
            if (Actions(lever) != null)
            {
                GameObject obj = Actions(lever);
                if (obj.GetComponent<BaseButton>().Type == InteractionType.Lever)
                    obj.GetComponent<BaseButton>().Toggle();
            }
        }
        else if (Input.GetAxisRaw(inputString) == 0)
        {
            axisInUse = false;
        }
    }

    private GameObject Actions(string[] tags)
    {
       ;
       
        bool blocked = false;
        Collider2D[] allObjs = Physics2D.OverlapCircleAll(transform.position, range);
        {
            for (int t = 0; t < tags.Length; t++)
            {
                if (blocked)
                    break;

                for (int i = 0; i < allObjs.Length; i++)
                {
                    isCorrectObj = false;


                    if (allObjs[i].tag == tags[t])
                    {
                        isCorrectObj = true;
                    }


                    if (isCorrectObj)
                    {
                        // RAYCAST TO CHECK WALL

                        float distance = Vector2.Distance(transform.position, allObjs[i].transform.position);
                        Vector2 direction = (allObjs[i].transform.position - transform.position).normalized;
                        Debug.DrawRay(transform.position, direction, Color.red);
                        // (transform.position + new Vector3((rayOffset + 0.1f)
                        // (transform.position + new Vector3((rayOffSetX - 0.1f) * directionMultiplier.x, 0, 0)
                        RaycastHit2D[] objHit = Physics2D.RaycastAll(transform.position, direction, distance);


                        for (int l = 0; l < objHit.Length; l++)
                        {
                            if (blocked)
                                break;

                            for (int j = 0; j < walls.Length; j++)
                            {
                                if (objHit[l].transform.tag == walls[j])
                                {
                                    blocked = true;
                                    break;
                                }
                            }
                        }

                        if (!blocked)
                        {
                            /*
                            if (isKey)
                            {
                                GetComponent<Pickup>().PickItUp(allObjs[i].gameObject);
                                isWielding = true;
                            }
                            else if (isObjHit)
                            {


                                if (allObjs[i].GetComponent<BaseButton>().Type == InteractionType.Lever)
                                    allObjs[i].GetComponent<BaseButton>().Toggle();
                            }
                            */
                            return (allObjs[i].gameObject);

                            ;
                        }
                    }
                }
            }
            return null;
        }
    }

}

