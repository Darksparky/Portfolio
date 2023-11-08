using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Attack : MonoBehaviour
{
    public GameObject fire;
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
            if (other.gameObject.GetComponent<Enemy>().isAlive)
            {
                other.gameObject.GetComponent<Enemy>().Die();
                gameObject.GetComponent<AudioSource>().Play();
                Instantiate(fire, other.ClosestPointOnBounds(gameObject.transform.position), Quaternion.identity);
            }
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            //Kill the enemy
            if (other.gameObject.GetComponent<Enemy>().isAlive)
            {
                other.gameObject.GetComponent<Enemy>().Die();
                gameObject.GetComponent<AudioSource>().Play();
                Instantiate(fire, other.ClosestPointOnBounds(gameObject.transform.position), Quaternion.identity);
            }

        }
    }
}
