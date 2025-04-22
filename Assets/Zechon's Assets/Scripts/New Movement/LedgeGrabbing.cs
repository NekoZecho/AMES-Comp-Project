using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [Header("Refrences")]
    public Player_Movement pm;
    public Transform orientation;
    public Rigidbody rb;
    [SerializeField]
    float castPosHeight;
    public Camera cam;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDist;
    public float minTOnLedge;
    private float tOnLedge;
    public bool holding;

    [Header("Ledge Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("Exiting")]
    public bool exitingLedge;
    public float exitLedgeTime;
    private float exitLedgeT;

    [Header("Ledge Detection")]
    public float ledgeDetectLength;
    public float ledgeSphereCastRad;
    public LayerMask theseAreLedges;
    Vector3 castPos;

    private Transform lastLedge;
    private Transform curLedge;

    private RaycastHit ledgeHit;

    void Update()
    {
        defineCastPos();
        LedgeDetection();
        SubStateMachine();
    }
    
    private void defineCastPos()
    {
        castPos = new Vector3(transform.position.x, transform.position.y + castPosHeight, transform.position.z);
    }

    private void SubStateMachine()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");

        bool anyInputPress = hInput != 0 || vInput != 0;

        //State Numero Uno (Number one lol)
        if (holding)
        {
            FreezeRBOnLedge();

            tOnLedge += Time.deltaTime;

            if (tOnLedge > minTOnLedge && anyInputPress)
            {
                ExitLedgeHold();
            }

            if (Input.GetKeyDown(jumpKey))
            {
                LedgeJump();
            }
        }

        //Exiting, aka state 2
        else if (exitingLedge)
        {
            if(exitLedgeT > 0)
            {
                exitLedgeT -= Time.deltaTime;
            }

            else
            {
                exitingLedge = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(castPos, .2f);
        Gizmos.DrawLine(castPos, orientation.forward);
        Gizmos.DrawLine(cam.transform.position, cam.transform.forward);
    }

    //checks for ledges, simple enough
    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(castPos, ledgeSphereCastRad, orientation.forward, out ledgeHit, ledgeDetectLength, theseAreLedges);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDist && !holding)
        {
            EnterLedgeHold();
        }
    }

    private void LedgeJump()
    {
        ExitLedgeHold();

        Invoke(nameof(DelayedJumpF), 0.05f);
    }
    
    private void DelayedJumpF()
    {
        Vector3 forceToAdd = orientation.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpForwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private void EnterLedgeHold()
    {
        holding = true;

        pm.unlimited = true;
        pm.restricted = true;

        curLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private void FreezeRBOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = curLedge.position - castPos;
        float distanceToLedge = Vector3.Distance(castPos, curLedge.position);

        //move da player to da ledge
        if (distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < moveToLedgeSpeed)
            {
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
            }
        }

        //Hold on
        else
        {
            if (!pm.freeze)
            {
                pm.freeze = true;
            }
            if (pm.unlimited)
            {
                pm.unlimited = false;
            }
        }

        //exit if something goes awry
        if (distanceToLedge > maxLedgeGrabDist) ExitLedgeHold();
    }

    private void ExitLedgeHold()
    {
        pm.restricted = false;
        pm.freeze = false;

        exitingLedge = true;
        exitLedgeT = exitLedgeTime;

        holding = false;
        tOnLedge = 0f;

        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}
