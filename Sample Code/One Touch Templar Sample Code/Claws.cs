using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claws : MonoBehaviour
{
    
    public GameObject parentObject; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (parentObject.GetComponent<Enemy>().isAlive&&gameObject.GetComponent<BoxCollider>().enabled == false)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        else if(parentObject.GetComponent<Enemy>().isAlive == false && gameObject.GetComponent<BoxCollider>().enabled == true)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public GameObject getParent()
    {
        return parentObject;
    }
}
