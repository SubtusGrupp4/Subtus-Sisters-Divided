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

    private bool isLever;
    private bool isKey;

    private bool isWielding;
    private bool axisInUse = false;

    PlayerController playerCont;

    // Use this for initialization
    void Awake()
    {
        playerCont = GetComponent<PlayerController>();

      //  inputString += playerCont.controllerCode;
       
        if (playerCont.Player == Controller.Player1)
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
        Actions();
    }

    void Actions()
    {
        //   lastValue = Input.GetAxisRaw(inputString);

        // input manger, button 4??
        Debug.Log("Inputmadad " + inputString);

        if (Input.GetAxisRaw(inputString) > 0 && !axisInUse)
        {
            //   ltValue *= -1 > 0 ? 1 : 0;

            axisInUse = true;
            GetComponent<Pickup>().DropIt();

            bool blocked = false;
            Collider2D[] allObjs = Physics2D.OverlapCircleAll(transform.position, range);
            {
                for (int t = 0; t < pickUp.Length; t++)
                {
                    if (blocked)
                        break;

                    for (int i = 0; i < allObjs.Length; i++)
                    {
                        isLever = false;
                        isKey = false;

                        if (allObjs[i].tag == pickUp[t])
                        {
                            isLever = false;
                            isKey = true;
                        }
                        else if (allObjs[i].tag == lever[t])
                        {
                            isLever = true;
                            isKey = false;
                        }

                        if (isKey || isLever)
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
                                if (isKey)
                                {
                                    GetComponent<Pickup>().PickItUp(allObjs[i].gameObject);
                                    isWielding = true;
                                }
                                else if (isLever)
                                {


                                    if (allObjs[i].GetComponent<BaseButton>().Type == InteractionType.Lever)
                                        allObjs[i].GetComponent<BaseButton>().Toggle();
                                }


                                break;
                            }
                        }
                    }
                }
            }
        }

        else if (Input.GetAxisRaw(inputString) == 0)
        {
            axisInUse = false;
        }
    }
}

