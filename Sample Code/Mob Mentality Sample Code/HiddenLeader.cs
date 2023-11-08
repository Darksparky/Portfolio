using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In mob mentality I attached this script to a hidden game object and provided direct control only to the invisible object. 
public class HiddenLeader : MonoBehaviour
{
    public GameObject unitLeader;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(unitLeader == null){
            Destroy(gameObject);
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }

    public void Move(Vector3 vector, float speed)
    {
        //transform.Translate((vector * speed * Time.deltaTime));
        transform.position = transform.position + (vector * speed *Time.deltaTime);
    } 
    

}
