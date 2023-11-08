using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    ParticleSystem pS; 

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    { 
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //This should check if the Enemy script is attatched to the other object
        if (other.gameObject.GetComponent<Enemy>())
        {
            //Kill the enemy
            other.gameObject.GetComponent<Enemy>().Die();
        }
    }
}
