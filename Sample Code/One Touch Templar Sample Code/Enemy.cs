using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Audio;


public class Enemy : MonoBehaviour
{
    public GameObject player;
    Vector3 direction = new Vector3();
    NavMeshAgent agent;
    Animator animator;
    bool canAttack = true;
    public bool inRange = false;
    public bool isAttacking = false;
    public float moveForce = 5;
    WaveManager waveManager;
    public GameObject AttackVFX;
    private GameObject VFXinstance;
    public Transform hand;
    public bool isAlive = true;
    public AudioSource deathSounds;
    public AudioSource footsteps;
    public GameObject fireFX;
    public GameObject hammerAttack;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        if(waveManager == null)
        {
            waveManager = WaveManager.Instance;
            waveManager.EnemyList.Add(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAlive)
        {
            if (player != null)
            {
                agent.destination = player.transform.position;
                animator.SetFloat("speed", agent.velocity.magnitude * 1.2f);
                
                //try and play the sound of footsteps based on speed of movement
                footsteps.pitch = agent.velocity.magnitude * 1.2f;
               
                //Debug.Log(gameObject.name + " velocity.magnitude = " + agent.velocity.magnitude.ToString());
                animator.SetFloat("angularspeed", agent.angularSpeed);

                if (inRange == true && canAttack == true)
                {
                    animator.SetTrigger("attack");
                }
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        

       
    }
    public void OnRange(Collider other)
    {
        if (isAlive)
        {
            Debug.Log("Entered enemy trigger");
            //If the player is the one that is inside the trigger
            if (other == player.GetComponent<CapsuleCollider>())
            {
                inRange = true;
                Debug.Log("Enemy in range to attack player");
            }
        }

    }

    public void OffRange(Collider other)
    {
        if (isAlive)
        {
            if (other == player.GetComponent<CapsuleCollider>())
            {
                inRange = false;
                Debug.Log("Enemy out of range to attack player");
            }
        }

    }
    public void AttackStart()
    {
        Debug.Log("attackstart was called");
        canAttack = false;
        isAttacking = true;
        StartCoroutine(Attacking());
        
    }
    public void AttackEnd()
    {
        Debug.Log("attackend was called");
        animator.ResetTrigger("attack");
        canAttack = true;
        isAttacking = false;
        Destroy(VFXinstance);
    }
    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(5.0f);
        float elapsedTime = 0;
        Vector3 des = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);
        while (elapsedTime < 6)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, des, elapsedTime / 6);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }
    public void Die()
    {
        isAlive = false;
        animator.SetTrigger("death");
        agent.destination = gameObject.transform.position;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        deathSounds.Play();
        if (waveManager.EnemyList.Contains(gameObject))
        {
           waveManager.EnemyList.Remove(gameObject);
        }
        else
        {
            Debug.Log("Enemy that was destroyed was not in the enemy list");
        }
        StartCoroutine(FadeAway());
        
    }
   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Attack>()||collision.gameObject.GetComponentInChildren<Attack>())
        {
            Debug.Log("Enemy Collision event with player attack");
            if (fireFX != null)
            {
                int num = collision.contactCount;
                for (int i = 0; i < num; i++)
                {
                    Instantiate(fireFX, collision.GetContact(i).point, Quaternion.identity);
                }
            }
        }
    }*/

    IEnumerator Attacking()
    {
        if (isAlive && inRange)
        {
            float timeElapsed = 0;
            bool created = false;
            AnimatorClipInfo[] animatorClipInfo;
            animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            float length = animatorClipInfo[0].clip.length;
            while (timeElapsed < length)
            {
                if (timeElapsed > length / 2)
                {
                    if (created == false)
                    {
                        VFXinstance = Instantiate(AttackVFX, hand.position, Quaternion.Euler(0, -270, 90));
                        created = true;
                    }

                }
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        yield return null;
    }
}
