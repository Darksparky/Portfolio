using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// This is the player controlling script for my capston project "minutes to map"
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController player;
    CharacterController characterController;
    public InteractionManager interactionManager;
    public InventoryController inventoryController;
    public InterfaceManager interfaceManager;
    CameraController cameraController;
    public Animator animator; 
    public new GameObject camera;
    public float speed;
    public float vSpeed;
    float gravity = 9.8f;
    bool endSlerp = false;
    public bool test;
    public const float MAX_DROPANGLE = 55f;
    //Movement direction
    Vector3 directionVector;

    private void Awake()
    {
        if (player == null)
        {
            //DontDestroyOnLoad(gameObject);
            player = this;
        }
        else if (player != this)
        {
            Destroy(gameObject);
        }
        
    }


    void Start()
    {
        //For now these are on the same game object and can be found by getting components
        characterController = gameObject.GetComponent<CharacterController>();
        inventoryController = gameObject.GetComponent<InventoryController>();
        
    }

    /// <summary>
    /// This function is used to help ensure that the player stays locked to the side a downward slope
    /// </summary>
    /// <param name="vel"></param>
    /// <returns></returns>
    private Vector3 AdjustVelocityToSlope(Vector3 vel)
    {
        var ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hitinfo, 0.5f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
            var adjustedVelocity = slopeRotation * vel;
            if(adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        return vel;
    }

    void Update()
    {
        HandleMove();
    }


    public IEnumerator SlerpTowards(Vector3 target, float duration)
    {
        endSlerp = false;
        float current = 0f;
        var lookPos = target - transform.position;
        lookPos.y = 0;
        Quaternion targRot = Quaternion.LookRotation(lookPos);
        while (current < duration)
        {
            transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targRot, current/duration);
            current = current + Time.deltaTime;
            if (endSlerp == true)
            {
                endSlerp = false;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public void EndSlerpTowards()
    {
        endSlerp = true;
    }

    private void HandleMove()
    {
        Vector2 moveInput = InputManager.instance.moveInput;
        var forward = camera.transform.forward;
        var right = camera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (characterController.isGrounded)
        {
            vSpeed = 0;
            //could have made an if statement for jumping here though we didn't have jumping in the game
        }

        //apply gravity acceleration to vertical speed
        vSpeed -= gravity * 3f * Time.deltaTime;
        directionVector = (forward * moveInput.y + right * moveInput.x)*speed;        
        if (directionVector != Vector3.zero)
        {
            if (interfaceManager.isInDialogue)
            {
                directionVector = Vector3.zero;
            }
            else if(!interfaceManager.isInMenu){
                transform.forward = directionVector;
            }
        }

        /* 
        In the following lines I have left some debug code commented in. I had used this debug code to combine trial and error with math to understand 
        how to find the the correct adjusted slope for the player to walk on. Prior to adjusting the direction of movement 
        the player character would move in "steps" down a slope because it was essencially launching itself off the side of even very minor slopes.
        If I didn't get the math right it could have done weird things like walk on a slight angle down every slope.  
        */


        //Debug.DrawLine(transform.position, transform.position + directionVector, Color.blue);
        Vector3 slopeDirection = AdjustVelocityToSlope(directionVector);
        // Debug.DrawLine(transform.position, transform.position + slopeDirection, Color.red);
        Vector3 adjusted = new Vector3((transform.position + directionVector).x, (transform.position+slopeDirection).y, (transform.position+ directionVector).z);
        //Debug.DrawLine(adjusted, new Vector3(adjusted.x, adjusted.y + 5, adjusted.z));
        //Debug.DrawLine(transform.position+slopeDirection, adjusted, Color.green);
        var ray = new Ray(adjusted, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hitinfo, 10f)){
            adjusted = hitinfo.point;
        }
        //Debug.DrawLine(transform.position, adjusted, Color.magenta);
        adjusted = adjusted - transform.position;

        float dropangle = Vector3.Angle(adjusted, directionVector);

        //Debug.Log("drop ANGLE is " + dropangle);
        if(dropangle<= MAX_DROPANGLE)
        {
            directionVector = adjusted;
        }
        directionVector.y += vSpeed;
        if (!interfaceManager.isInMenu)
        {
            characterController.Move(directionVector * Time.deltaTime);
        }
        else
        {
            characterController.Move(Vector3.zero);
        }
        var rayForGround = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(rayForGround, out RaycastHit hI, 1f))
        {
            animator.SetFloat("IdleWalkRun", characterController.velocity.magnitude / (speed-1.5f));
        }
        else
        {
            animator.SetFloat("IdleWalkRun", 0f);
        }
    }


 }

