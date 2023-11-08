using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    enum PlayerState
    {
        Idle,
        Swinging,
        Throwing,
        Dead,
    }
    [SerializeField]
    PlayerState state;
    public Animator animator;
    public HealthController healthController;
    public GameObject Hammer;
    public GameObject HammerHead;
    public GameObject HammerVFX;
    public AudioSource hammerSound;
    public float rotationSpeed = 2;
    public float moveSpeed = 5;
    public GameObject bloodVFX;
    public GameObject teleportVFX;
    public GameObject explosionVFX;
    public GameObject sparkleTrailVFX;
    public GameObject lightningVFX;
    public GameObject vCam;
    public SoundList gruntList;
    public SoundList shoutList;
    //NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        state = PlayerState.Idle;
        animator = gameObject.GetComponent<Animator>();
        //agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if (state == PlayerState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.X)) //Only returns true on first press
            {
                Swing();
            }
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Swing();
                }
            }
        }

        if (state == PlayerState.Swinging)
        {
            if (Input.GetKeyUp(KeyCode.X)) //Returns on the frame that the key is let go
            {
                Throw();
            }
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    Throw();
                }
            }
        }

        if(state == PlayerState.Dead && healthController.health > 0)
        {
            animator.ResetTrigger("Dead");
            animator.SetTrigger("Alive");
            state = PlayerState.Idle;
        }

        if (healthController.health <= 0 && state != PlayerState.Dead)
        {
            //Really should check if playing the animation
            animator.ResetTrigger("Alive");
            animator.SetTrigger("Dead");
            state = PlayerState.Dead;
        }

        

    }

    /// <summary>
    /// Throws the hammer
    /// </summary>
    void Throw()
    {
        state = PlayerState.Throwing;
        //Get player position
        //Get position of hammer head
        //Create directional vector 
        //Start the throw coroutine by putting the parameters into the coroutine
        animator.SetBool("Swinging", false);
        StartCoroutine(Throwing());

    }

    /// <summary>
    /// Starts swinging the hammer
    /// </summary>
    void Swing()
    {
        state = PlayerState.Swinging;
        animator.SetBool("Swinging", true);
        StartCoroutine(Swinging());

    }

    /// <summary>
    /// Start Swinging 
    /// </summary>
    IEnumerator Swinging()
    {
        Debug.Log("started swinging");
        float z = Hammer.transform.rotation.z;
        hammerSound.pitch = 2f;
        while (state == PlayerState.Swinging)
        {

            z = rotationSpeed * Time.deltaTime;
            //Debug.Log(z);
            if (z > 360.0f)
            {
                z = 0.0f;
            }
            //Hammer.transform.localRotation = Quaternion.Euler(0,0,z);
            Hammer.transform.Rotate(new Vector3(0, z, 0), Space.Self);
            gameObject.transform.Rotate(new Vector3(0, z, 0), Space.Self);
            //rotate hammer around player
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// Throwing the hammer
    /// </summary>
    IEnumerator Throwing()
    {
        Debug.Log("started throwing");
        Vector3 hPos;
        Vector3 gPos;
        Vector3 direction = new(0, 0, 0);
        if (Hammer != null && HammerHead != null)
        {
            hPos = Hammer.transform.position;
            gPos = HammerHead.transform.position;
            direction = gPos - hPos;
            direction.Normalize();
        }
        else
        {
            Debug.Log("Hammer or HammerHead is null");
        }
        shoutList.Randomize();
        shoutList.Play();
        yield return new WaitForSeconds(0.33f);

        while (state == PlayerState.Throwing)
        {
            if (Input.GetKeyDown(KeyCode.X)) //Only returns true on first press
            {
                //change position of player to that of hammer
                //Debug.Log("Blink");
                if (Hammer != null)
                {
                    StartCoroutine(Teleport());
                    //agent.destination = Hammer.transform.position;
                }
                else
                {
                    Debug.Log("Hammer is Null");
                }


            }
            if (Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    //change position of player to that of hammer
                    //Debug.Log("Blink");
                    if (Hammer != null)
                    {
                        StartCoroutine(Teleport());
                        //agent.destination = Hammer.transform.position;
                    }
                    else
                    {
                        Debug.Log("Hammer is Null");
                    }
                }
            }

            //Hammer.transform.Translate(direction * Time.deltaTime);
            Hammer.transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, HammerHead.transform);
            //Keep translating the hammer away from the player
            yield return null;
        }
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Claws>()!= null)
        {
            GameObject attacker = other.gameObject.GetComponent<Claws>().getParent();
            if (attacker.GetComponent<Enemy>().isAttacking)
            {
                //needs a way to insure only one tick of damage is dealt per attack but I can't deal with this right now
                healthController.ReduceHealth();
                gruntList.Randomize();
                gruntList.Play();
                if(bloodVFX != null)
                {
                    Vector3 dr = other.gameObject.transform.position - other.ClosestPoint(gameObject.transform.position);
                    Instantiate(bloodVFX, other.ClosestPoint(gameObject.transform.position), Quaternion.Euler(dr));
                    vCam.GetComponent<CameraController>().ShakeMe(2f,1f);
                }
            }
        }
    }
    
    /// <summary>
    /// Create an instance of the player's teleport animation then move the player after the animation. 
    /// </summary>
    IEnumerator Teleport()
    {
        bool hasTeleported = false;
        bool hasDeletedVFX = false;
        bool thunder = false;
        //check to ensure there are no null values for the objects 
        if (HammerVFX != null && teleportVFX != null) 
        {
            //check to ensure that the teleportVFX has a particle system
            if (teleportVFX.GetComponent<ParticleSystem>())
            {
                GameObject tvfx = Instantiate(teleportVFX, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z), Quaternion.identity);
                tvfx.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.4f, gameObject.transform.position.z);
                GameObject trail = Instantiate(sparkleTrailVFX, new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+1,gameObject.transform.position.z), Quaternion.identity);
                while (hasDeletedVFX == false)
                {
                    while(hasTeleported == false)
                    {
                        while (tvfx.GetComponent<ParticleSystem>().time < 0.9f)
                        {

                            trail.transform.position = Vector3.Lerp(trail.transform.position, new Vector3(Hammer.transform.position.x, Hammer.transform.position.y + 1, Hammer.transform.position.z), tvfx.GetComponent<ParticleSystem>().time / 0.9f);
                            if (tvfx.GetComponent<ParticleSystem>().time > 0.8f && thunder == false)
                            {
                                thunder = true;
                                GameObject lightning = Instantiate(lightningVFX,Hammer.transform.position, Quaternion.identity);
                            }
                            yield return new WaitForFixedUpdate();
                        }
                        
                        //Debug.Log("tvfx time = " + tvfx.GetComponent<ParticleSystem>().time);
                        if (tvfx.GetComponent<ParticleSystem>().time >0.9f)
                        {
                            //move the player to the position of the fireball/hammer
                            gameObject.transform.position = Hammer.transform.position;
                            StartCoroutine(Attack());
                            hasTeleported = true;
                            hammerSound.pitch = 0.5f;
                            state = PlayerState.Idle;
                            yield return new WaitForFixedUpdate();
                        }
                        yield return new WaitForFixedUpdate();
                    }
                    if (tvfx.GetComponent<ParticleSystem>().isStopped)
                    {
                        Destroy(tvfx);
                        Destroy(trail);
                        hasDeletedVFX = true;
                        yield return new WaitForFixedUpdate();
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
    }
    
    public int playerStateToInt()
    {
        int v = 0;
        switch (state)
        {
            case PlayerState.Idle:
                v = 0;
                break;
            case PlayerState.Swinging:
                v = 1;
                break;
            case PlayerState.Throwing:
                v = 2;
                break;
            case PlayerState.Dead:
                v = 3;
                break;
        }
        return v;
    }
    IEnumerator Attack()
    {
        bool hasDeletedVFX = false;
        if(explosionVFX != null)
        {
            GameObject avfx = Instantiate(explosionVFX, Hammer.transform.position, Quaternion.identity);
            vCam.GetComponent<CameraController>().ShakeMe(2f,1f);
            while (hasDeletedVFX == false)
            {
                if(avfx.GetComponent<ParticleSystem>().time == 1)
                {
                    Debug.Log("explosion vfx time = 1");
                    Destroy(avfx);
                    hasDeletedVFX=true;
                }
                yield return new WaitForFixedUpdate();
            }

        }
        yield return new WaitForFixedUpdate();
    }
}
