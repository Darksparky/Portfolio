using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations;

//The following script was made for mob mentality, and is the unit level script. Note that the player doesn't have direct control of the units
public class Villager : MonoBehaviour
{
    /// <summary> The unit group that this unit belongs to </summary>
    private UnitGroup unitGroup;
    public PlayerInput playerInput;
    GroupManager groupManager;
    GameManager gameManager;
    NavMeshAgent agent;
    public bool arrived = false;
    /// <summary> The radius the unit will see the enemy and start attacking it </summary>
    public float attackRange;

    public float stopDis = 1.2f;


    /// <summary> The radius an idle unit will see the enemy and start running from it </summary>
    public float fleeRange;

    /// <summary> The enemy the units will use to check distance </summary>
    public Transform enemy;

    /// <summary> The villagers attack script </summary>
    VillagerAttack villagerAttack;

    public GameObject follower;

    public GameObject highlighted;

    public GameObject leader;

    EmoteSystem emoteSystem;

    public Animator villagerAnimator;

    public float rallyCooldown = 0.2f;
    private float tempRallyCooldown;

    public AudioSource rallySound;
    public AudioSource warcrySound;

    bool follow = false;

    public RotationConstraint leaderMapIndicator;
    public RotationConstraint villagerMapIndicator;

    ConstraintSource conSource;

    //For Debugging
    public TMPro.TextMeshProUGUI textMesh;


    [SerializeField]
    public float moving = 0;

    public List<Villager> neighbours = new List<Villager>();
    /// <summary>
    /// The states which a villager could be in.
    /// </summary>
    public enum State
    {
        idling,
        patroling,
        fleeing,
        leading,
        following,
        attacking,
        dead
    }

    ///<summary> The state that the villager is in. </summary>
    [SerializeField] private State state = State.idling;

    private void Awake()
    {
        
        enemy = GameObject.FindGameObjectsWithTag("Enemy")[0].transform;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        var mapParent = GameObject.Find("MapCameraParent");
        conSource.sourceTransform = mapParent.transform;
        conSource.weight = 1;
        leaderMapIndicator.AddSource(conSource);
        villagerMapIndicator.AddSource(conSource);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerInput>();
        groupManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GroupManager>();
        villagerAttack = GetComponent<VillagerAttack>();
        agent = GetComponent<NavMeshAgent>();
        emoteSystem = GetComponent<EmoteSystem>();
        neighbours.Clear();
        follower.SetActive(true);
        leader.SetActive(false);
        enemy = GameObject.FindGameObjectsWithTag("Enemy")[0].transform; 
    }

    // Update is called once per frame
    void Update()
    {


        //moving = Mathf.Abs(agent.velocity.x) + Mathf.Abs(agent.velocity.z);
        highlighted.SetActive(gameManager.IsOverview());
        moving = agent.velocity.magnitude;
        villagerAnimator.SetFloat("SpeedVal", moving);
        float distanceLeft = agent.remainingDistance;
        if(distanceLeft!=Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            arrived = true;
        }
        else
        {
            arrived = false;
        }
        /* if(moving <= 0.15f){
             villagerAnimator.SetBool("Run", false);
             villagerAnimator.SetBool("Walk", false);
         }
         else if(moving <= 1f){
            villagerAnimator.SetBool("Run", false);
            villagerAnimator.SetBool("Walk", true);
         }
         else{
             villagerAnimator.SetBool("Run", true);
             villagerAnimator.SetBool("Walk", false);
         }*/

        if (textMesh != null)
        {
            //textMesh.text = agent.destination.ToString();
          
            if (state == State.leading && textMesh.text != "leader")
            {
                textMesh.text = "leader";
                highlighted.SetActive(false);
                //follower.SetActive(false);
                //leader.SetActive(true);
                //transform.localScale = new Vector3(0.8f,0.8f,0.8f);
            }else if (state == State.following && textMesh.text != "follower")
            {
                textMesh.text = "follower";
                emoteSystem.Emote("Follow");
            }else if (state == State.attacking && textMesh.text != "attacking")
            {
                textMesh.text = "attacking";
                emoteSystem.Emote("Angry");
            }else if (state == State.fleeing && textMesh.text != "fleeing")
            {
                emoteSystem.Emote("Fear");
                textMesh.text = "fleeing";
            } else if(state == State.idling && textMesh.text != "idle")
            {
                textMesh.text = "idle";
            }
        }
        if (arrived)
        {
            textMesh.text = "Arrived";
        }
        else if (!arrived)
        {
            textMesh.text = "Moving";
        }


        neighbours.RemoveAll(item => item == null);

        Transform closestDead = FindClosestDeadBody().transform;
        if(tempRallyCooldown > 0){
            tempRallyCooldown -= Time.deltaTime;
        }
        switch (state)
        {
            case State.idling:
                follow = false;
                agent.speed = 3;
                agent.acceleration = 5;
                if(Vector3.Distance(transform.position,closestDead.position) <= fleeRange){
                    emoteSystem.Emote("Sad");
                }
                if(Vector3.Distance(transform.position,enemy.transform.position) <= fleeRange){
                    state = State.fleeing;
                }
                break;
            case State.patroling:
                break;
            case State.fleeing:
                if(!follow){
                    agent.speed = 2;
                    agent.acceleration = 5;
                }
                
                if(Vector3.Distance(transform.position,enemy.transform.position) > fleeRange*2 && !follow){
                    state = State.idling;
                }
                else if(Vector3.Distance(transform.position,enemy.transform.position) > fleeRange && follow){
                    state = State.following;
                }

                Vector3 dirToEnemy = transform.position - enemy.transform.position;
                Vector3 newPos = transform.position + dirToEnemy;
                agent.SetDestination(newPos);

                break;
            case State.leading:
                agent.speed = 8;
                agent.acceleration = 15;
                if(Vector3.Distance(transform.position,closestDead.position) <= fleeRange){
                    emoteSystem.Emote("Sad");
                }
                if(agent.avoidancePriority!= 25)
                {
                    agent.avoidancePriority = 25;
                }

                break;
            case State.following:
                follow = true;
                agent.speed = 8;
                agent.acceleration = 15;
                if(Vector3.Distance(transform.position,closestDead.position) <= fleeRange){
                    emoteSystem.Emote("Sad");
                }
                if(unitGroup.units.Count > 5){
                    if(Vector3.Distance(transform.position,enemy.transform.position) <= attackRange){
                        state = State.attacking;
                    }
                }
                else{
                    if(Vector3.Distance(transform.position,enemy.transform.position) <= attackRange){
                        state = State.fleeing;
                    }
                }
                
                Follow();
                break;
            case State.attacking:
                if(Vector3.Distance(transform.position,enemy.transform.position) > attackRange){
                    state = State.following;
                }
                villagerAttack.Attack(enemy);
                Follow();
                break;
            case State.dead:
                break;
        }

    }

    public GameObject FindClosestDeadBody()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Dead");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    /// <summary>
    /// Follows the leader. 
    /// </summary>
    private void Follow(){
        if (agent.avoidancePriority != 99)
                {
                    agent.avoidancePriority = 99;
                }
        Vector4 cumulative = new Vector4();
        Quaternion avg = Quaternion.identity;
        if (neighbours.Count == 1)
        {
                avg = Quaternion.Slerp(gameObject.transform.rotation, neighbours[0].transform.rotation, 0.5f);
        }
        else if (neighbours.Count >= 1)
        {
            foreach (Villager item in neighbours)
            {
                if(item!= null && unitGroup.CheckForContains(item.gameObject) == true)
                    avg = MyMath.AverageQuaternion(ref cumulative, item.transform.rotation, gameObject.transform.rotation, neighbours.Count);
            }
                gameObject.transform.rotation = avg;
        }
    }
    
    

    /// <summary>
    /// Sets the group that the villager belongs to. 
    /// </summary>
    /// <param name="unitGroup">The group the villager belongs to. </param>
    public void SetUnitGroup(UnitGroup unitGroup)
    {
        this.unitGroup = unitGroup;
    }

    /// <returns> The group that the unit belongs to.</returns>
    public UnitGroup GetUnitGroup()
    {
        return unitGroup;
    }

    /// <summary>
    /// Adds a villager to the villager's list of neighbours if it is not already on the list.
    /// </summary>
    /// <param name="toAdd">the new villager to add to the list of neighbours</param>
    public void AddNeighbour(Villager toAdd)
    {
        if (!neighbours.Contains(toAdd))
        {
            neighbours.Add(toAdd);
        }
    }
    /// <summary>
    /// Removes a villager from the villager's list of neighbours if it is on the list
    /// </summary>
    /// <param name="toRemove">The villager that is no longer a neighbour</param>
    public void RemoveNeighbour(Villager toRemove)
    {
        //if (neighbours.Contains(toRemove))
        //{
        neighbours.Remove(toRemove);
        // }
    }

    //for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fleeRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }



    /// <summary>
    /// Checks to see if the villager is leading and then removes itself from the playerInput class leader
    /// </summary>
    private void CheckAndRemoveLeadership()
    {
        if (state == State.leading)
        {
            playerInput.RemoveUnitIfPresent(gameObject);
        }
    }

    public void SetToIdle()
    {
        CheckAndRemoveLeadership();
        unitGroup = null;
        state = State.idling;
    }

    public void SetToPatrol()
    {
        CheckAndRemoveLeadership();
        state = State.patroling;
    }

    public void SetToLeader()
    {
        state = State.leading;
        playerInput.SetUnit(gameObject);
    }

    public void SetToFollow()
    {
        CheckAndRemoveLeadership();
        state = State.following;
    }

    public void SetToDead()
    {
        CheckAndRemoveLeadership();
        state = State.dead;
    }

    public bool IsLeading()
    {
        return state == State.leading ? true : false;
    }

    public bool IsFollowing()
    {
        return state == State.following ? true : false;
    }

    public bool IsDead()
    {
        return state == State.dead ? true : false;
    }

    public bool IsAttacking()
    {
        return state == State.attacking ? true : false;
    }

    public bool IsIdling()
    {
        return state == State.idling ? true : false;
    }

    public bool IsFleeing()
    {
        return state == State.fleeing ? true : false;
    }
    /// <summary>
    /// Checks if neighbours within stopping distance are stopped.
    /// </summary>
    /// <returns></returns>
    public bool CheckForStopped()
    {
        //return value r
        bool r = false;
        
        foreach(Villager item in neighbours)
        {
            if(item != null){
                float dist;

                Vector3 dir = gameObject.transform.position - item.gameObject.transform.position;
                
                dist = dir.magnitude;
                //Debug.Log($"distance to neighbour = {dist}"); 
                if(dist <= stopDis)
                {
                    if (item.arrived)
                    {
                        r = true;
                    }
                }
                else if(item.IsLeading()){

                }
            }

        }
        if (r)
        {
            //Debug.Log("CheckForStopped = true");
        }
        else
        {
            //Debug.Log("CheckForStopped = false");
        }


        return r;
    }

    public void Rally()
    {
       
        if (!IsIdling() && !IsFleeing() && tempRallyCooldown <= 0)
        {
            emoteSystem.Emote("Beg");
            tempRallyCooldown = rallyCooldown;
            Debug.Log("Rally!");
            villagerAnimator.SetTrigger("Wave");
            if(unitGroup.units.Count < 5)
            {
                for(int i = 1; i <= unitGroup.units.Count; i++)
                {
                    StartCoroutine(RecruitYell());
                }
            } else
            {
                warcrySound.pitch = Random.Range(0.95f, 1.1f);
                warcrySound.Play();
            }
            if (neighbours.Count > 0)
            {
                Debug.Assert(unitGroup != null);
                
                foreach (Villager item in neighbours)
                {
                    if (item != null)
                    {
                        if (item.GetUnitGroup() == unitGroup && unitGroup != null)
                        {
                            //neighbour is already in your group
                            //Debug.Log("neighbour is already in your group");
                        }
                        else
                        {
                            if (item.GetUnitGroup() == null)
                            {
                                unitGroup.AddUnit(item.gameObject);
                                item.SetToFollow();
                                item.SetUnitGroup(unitGroup);
                            }
                            else if (item.GetUnitGroup() != null)
                            {
                                Debug.Log("will combine");
                                groupManager.CombineGroups(unitGroup, item.GetUnitGroup(), gameObject, unitGroup.hiddenLeader);
                            }
                        }
                    }
                }
            }
        }
    }
    IEnumerator RecruitYell()
    {
        yield return new WaitForSeconds(Random.Range(0.02f, 0.3f));
        rallySound.volume = (0.1f / Mathf.Min(unitGroup.units.Count, 5)) + 0.1f;
        rallySound.pitch = Random.Range(0.9f, 1.0f);
        rallySound.PlayOneShot(rallySound.clip);
        yield return null;
    }
}

