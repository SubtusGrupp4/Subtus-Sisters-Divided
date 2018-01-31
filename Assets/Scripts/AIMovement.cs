using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementEnum
{
    Patrolling, Stalking, OneDirBounce, Idle
}

public class AIMovement : MonoBehaviour
{
    private const float patrollGrace = 0.1f;

    [Header("Interactions")]

    [GiveTag]
    public string[] EngageOn = new string[] { };
    [GiveTag]
    public string[] BounceOn = new string[] { };
    [GiveTag]
    public string[] WalkOn = new string[] { };


    [Header("States")]

    public MovementEnum StartState;
    public MovementEnum EngagedState;
    private MovementEnum currentState;
    private MovementEnum savedState;

    [Header("Stats")]

    public float Speed;
    public float AggroRange;
    public float StepRange;
    private bool isDead = false;


    [Header("Check Points")]

    public List<GameObject> CheckPoints = new List<GameObject>();
    private List<Vector2> checkPointPos = new List<Vector2>();
    private List<bool> checkPointCheck = new List<bool>();
    private bool reverse = false;
    private int checkPointIndex = 0;

    private bool isFalling;
    private bool engaged;

    private GameObject target;

    private bool bounce;
    private Vector2 directionMultiplier; // incase u want to reverse gravity or something

    private new Rigidbody2D rigidbody2D;
    private List<GameObject> allTargets = new List<GameObject>();

    private RaycastHit2D[] objHit;
    private float rayDistance;
    private float rayOffset;
    private float slopeRayOffset;
    private int flipValue = 1;

    void Start()
    {
        currentState = StartState;
        rigidbody2D = GetComponent<Rigidbody2D>();

        for (int i = 0; i < CheckPoints.Count; i++)
        {
            checkPointPos.Add(CheckPoints[i].transform.position);
            checkPointCheck.Add(false);
        }


        directionMultiplier = new Vector2(1, 0);

        rayDistance = Mathf.Pow((GetComponent<BoxCollider2D>().size.y * GetComponent<Transform>().localScale.y), 2) + // X^2
           Mathf.Pow((GetComponent<BoxCollider2D>().size.x * GetComponent<Transform>().localScale.x), 2);             // Y^2 

        rayDistance = Mathf.Sqrt(rayDistance);
        rayDistance += 0.05f; // offSet.

        rayOffset = GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2;

        slopeRayOffset = rayOffset;

    }




    void Update()
    {
        if (!GetComponent<Rigidbody2D>())
            isDead = true;


        if (!isDead)
        {
            // if (EngageOn != null)
            if (engaged == false || target == null)
            {
                CheckEngagement();
                Debug.Log("YES");
            }

            //  CheckFalling();

        }
    }

    private void FixedUpdate()
    {
        if (!isDead) //&& isFalling)
        {
            Move();
        }
    }

    public void Freeze()
    {
        savedState = currentState;
        currentState = MovementEnum.Idle;
    }

    public void UnFreeze()
    {
        currentState = savedState;
    }

    public void Die()
    {
        isDead = true;
    }

    private void CheckFalling()
    {
        isFalling = true;
        objHit = Physics2D.RaycastAll(transform.position, -Vector2.up * flipValue, 1);

        Debug.DrawRay(transform.position, -Vector2.up * flipValue, Color.blue);

        for (int i = 0; i < WalkOn.Length; i++)
        {
            for (int j = 0; j < objHit.Length; i++)
            {
                if (!objHit[j].transform)
                    continue;

                if (objHit[j].transform.tag == WalkOn[i])
                {
                    isFalling = false;
                }
            }
        }


    }

    private void CheckEngagement()
    {
        allTargets.Clear(); // clear the target list, incase another target is added or removed etc...
        if (EngageOn.Length >= 1)
            for (int i = 0; i < EngageOn.Length; i++)
                allTargets.AddRange(GameObject.FindGameObjectsWithTag(EngageOn[i]));

        float tempDistance = AggroRange;
        GameObject tempTarget = null;

        for (int i = 0; i < allTargets.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, allTargets[i].transform.position);

            if (distance > AggroRange)
                continue;

            if (distance <= tempDistance)
            {
                tempDistance = distance;
                tempTarget = allTargets[i];
            }
        }
        target = tempTarget;
        if (target != null)
        {
            engaged = true;
            currentState = EngagedState;
        }
    }

    private void Move()
    {
        bounce = false;
        if (currentState == MovementEnum.Stalking)
        {
            float X = target.transform.position.x - transform.position.x; // positive value = right, negative = left

            if (X > 0)
                rigidbody2D.velocity = new Vector2(1 * Speed, rigidbody2D.velocity.y);

            if (X < 0)
                rigidbody2D.velocity = new Vector2(-1 * Speed, rigidbody2D.velocity.y);

            //    rigidbody2D.velocity = new Vector2(GetComponent<Rigidbody2D>, GetComponent<Rigidbody2D>().velocity.y);
        }

        if (currentState == MovementEnum.OneDirBounce)
        {


            bounce = true;
            // CheckFalling();
            if (!isFalling)
                rigidbody2D.velocity = new Vector2(Speed * directionMultiplier.x, rigidbody2D.velocity.y);


            if (CheckSlope() == true)
            {
                // DO NOTHING
                // Therefore keeping the same direction and speed as "inteded"
            }
            else if (CheckLedge())
                Bounce();


            if (CheckWall())
                Bounce();
        }

        if (currentState == MovementEnum.Patrolling)
        {
            if (Vector2.Distance(transform.position, checkPointPos[checkPointIndex]) <= patrollGrace)
            {
                if (reverse)
                    checkPointIndex--;

                else if (!reverse)
                    checkPointIndex++;
            }

            if (checkPointIndex == CheckPoints.Count - 1)
            {
                reverse = true;
            }
            else if (checkPointIndex == 0)
            {
                reverse = false;
            }

            Vector2 desiredDir = (checkPointPos[checkPointIndex] - (Vector2)transform.position).normalized;
             rigidbody2D.velocity = desiredDir * Speed;
          //  transform.Translate(desiredDir * Speed * Time.deltaTime);

        }

        if (currentState == MovementEnum.Idle)
        {
            if (rigidbody2D.velocity.x != 0)
                rigidbody2D.velocity = Vector2.zero;
        }

    }


    private bool CheckWall()
    {
        bool walls = false;

        objHit = Physics2D.RaycastAll(transform.position, new Vector2(directionMultiplier.x, 0), 1f);
        Debug.DrawRay(transform.position, new Vector2(directionMultiplier.x, 0), Color.green);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < BounceOn.Length; j++)
            {
                if (objHit[i].transform.tag == BounceOn[j])
                {
                    Debug.Log("normal X WALS" + Mathf.Abs(objHit[i].normal.x));

                    if ((Mathf.Abs(objHit[i].normal.x) > 0.8f))
                    {
                        walls = true;
                    }
                }
            }
        }


        return walls;
    }

    private bool CheckSlope()
    {
        bool slopes = false;

        objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffset + 1.1f) * directionMultiplier.x, -slopeRayOffset - 0.1f * flipValue, 0), -Vector2.right * directionMultiplier.x, StepRange * 3);

        Debug.DrawRay(transform.position + new Vector3((rayOffset + 1.1f) * directionMultiplier.x, -slopeRayOffset - 0.1f * flipValue), -Vector2.right * directionMultiplier.x, Color.blue);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < WalkOn.Length; j++)
            {
                if (objHit[i].transform.tag == WalkOn[j])
                {
                    if (Mathf.Abs(objHit[i].normal.x) > 0.1f && (Mathf.Abs(objHit[i].normal.x) < 0.8f))
                    {
                        slopes = true;
                        Debug.Log("normal X" + Mathf.Abs(objHit[i].normal.x));
                    }
                }
            }
        }


        return slopes;
    }

    private bool CheckLedge()
    {
        bool floors = true;

        objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffset + 0.1f) * directionMultiplier.x, 0, 0), -Vector2.up * flipValue, StepRange);

        Debug.DrawRay(transform.position + new Vector3((rayOffset + 0.1f) * directionMultiplier.x, 0, 0), -Vector2.up * flipValue, Color.red);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < WalkOn.Length; j++)
            {
                if (objHit[i].transform.tag == WalkOn[j])
                {
                    floors = false;
                }
            }
        }

        /*  if (objHit.Length <= 0)
              return true;
          else
              return false; */

        return floors;








        //  else
        //   Debug.Log("objhitlengt " + objHit.Length);




        /*for(int i = 0; i < objHit.Length; i++)
        {
            if(objHit[i].transform.gameObject == this.gameObject)
            {
                if(objHit)
            }
        }*/
        /*
        for (int i = 0; i < objHit.Length; i++)
        {
            

            if (objHit[i].transform.tag != BounceOn[0])
            {
                Debug.Log("AI hit" + objHit[i].transform.gameObject);
                directionMultiplier *= -1;
            }
        }*/
        // Incase we cant have boxcollider
        /*
        for(int i = 0; i < BounceOn.Length; i++)
        {

            for(int j = 0; j < objHit.Length; i++)
            {

            }


        }
           */
        // if(objHit)

    }

    private void Bounce()
    {
        directionMultiplier *= -1;
    }


}
