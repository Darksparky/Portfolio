//The following file was something I created to group units in mob mentality. ALL of the comments below were made at the time of making the file. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


//This allows unity to put the class in the inspector among other things
[System.Serializable]
public class UnitGroup 
{
    GroupManager groupManager;
    /// <summary> The leader is the transform the group will follow. </summary>
    public Transform hiddenLeader;
    
    [SerializeField]
    /// <summary> playerUnit is the Unit that the player is in control of - this isn't always in the UnitGroup</summary>
    public GameObject unitLeader;
    
    /// <summary> List of units in the group </summary>
    public List<GameObject> units = new List<GameObject>();

    bool oncePerL = false;

    public float reactionDistance = 0.3f; //If reaction distance is too large it will units will move really weird and stutter when they get too close to the leader. 
    

    /// <summary>
    /// Create a new unit group class with a hiddenLeader. 
    /// </summary>
    /// <param name="hiddenLeader">The hiddenLeader is the transorm the group will follow.</param>
    /// <param name="unitLeader">The unit that is designated as Leader</param>
    public UnitGroup(Transform hiddenLeader, GameObject unitLeader, GroupManager groupManager) {
        this.hiddenLeader = hiddenLeader;
        this.unitLeader = unitLeader;
        this.groupManager = groupManager;
        AddUnit(unitLeader);
    }

    public UnitGroup(Transform hiddenLeader, GameObject unitLeader, GroupManager groupManager, List<GameObject> units)
    {
        this.hiddenLeader = hiddenLeader;
        this.unitLeader = unitLeader;
        this.groupManager = groupManager;
        AddUnit(unitLeader);
        foreach(GameObject item in units)
        {
            AddUnit(item);
        }
    }
    
    ~UnitGroup()
    {
        units.Clear();
    }

    /// <summary>
    /// Adds a unit to the UnitGroup if it is not already in the group.
    /// </summary>
    /// <param name="toAdd">The unit you are adding to the group</param>
    public void AddUnit(GameObject toAdd)
    {
        if (!units.Contains(toAdd))
        {
            units.Add(toAdd);
        }
    }
    /// <summary>
    /// Removes a unit to the UnitGroup if it is in the group.
    /// </summary>
    /// <param name="toRemove">The unit you are removing from the group</param>
    public void RemoveUnit(GameObject toRemove)
    {
        if (units.Contains(toRemove))
        {
            units.Remove(toRemove);
        }
    }

    //NEEDS TO BE CALLED EVEY FRAME (UPDATE) BY GROUPMANAGER
    public void Update()
    {
        units.RemoveAll(item => item == null);
        if (unitLeader!=null)
        {
            UpdateUnitDestinations();
            if(oncePerL){
                oncePerL = false;
            }
            //units.RemoveAll(item => item == null);
        }
        else{
            foreach(GameObject item in units)
            {
                if(item != null){ 
                    item.GetComponent<Villager>().SetToIdle();
                }
                
            }
            if(!oncePerL){
                oncePerL = groupManager.lostGroupDialogue();
            }
            units.Clear();

        }
        
        if (units.Count <= 0)
        {
            Debug.Log("Destroying Unit Group");
            Debug.Assert(groupManager);
            groupManager.RemoveGroup(this);
        }
         
    }

    void UpdateUnitDestinations()
    {
        foreach (GameObject item in units)
        {
            /* 
             Goal for this:
                If item is not the leader and is within NavMeshAgent's stopping distance of a neighbour who is stopped
                then -> stop and make this the destination. 
              
             if the item is the leader then -> destination = hidden leader position

            if the item is not the leader then -> if the leader is moving && the destination 
            is further than a given distance then -> the destination = hiden leader position
             */
            if (item != null)
            {

                if (item == unitLeader)
                {

                    item.GetComponent<NavMeshAgent>().destination = hiddenLeader.position;
                    
                }

                if (item != unitLeader && item.GetComponent<Villager>().CheckForStopped() == true && unitLeader.GetComponent<Villager>().arrived == true)
                {
                    //item.GetComponent<NavMeshAgent>().ResetPath();
                    Debug.Log("Should stop because unit is stopped nearby");
                    item.GetComponent<NavMeshAgent>().destination = item.transform.position;
                }
                else if (item != unitLeader )//&& unitLeader.GetComponent<Villager>().arrived == false)
                {
                    //var dist = (hiddenLeader.position - item.transform.position).magnitude;
                    //if (dist > reactionDistance)
                   // {
                        //item.GetComponent<NavMeshAgent>().ResetPath();
                        item.GetComponent<NavMeshAgent>().destination = hiddenLeader.position;
                    //}
                    
                }

            }
        }

    }
    public void ChangeUnitLeader(GameObject newLeader)
    {
        unitLeader = newLeader;
        hiddenLeader.GetComponent<HiddenLeader>().unitLeader = newLeader;
    }
    
    public void StopDestinations()
    {
        foreach(GameObject item in units)
        {
            item.GetComponent<NavMeshAgent>().ResetPath();

        }
    }

    public bool CheckForContains(GameObject toCheck)
    {
        return units.Contains(toCheck);
    }
}
