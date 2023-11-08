using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isPlayerInside = false;
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
        if (other.gameObject.GetComponent<PlayerController>())
        {
            isPlayerInside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            isPlayerInside = false;
        }
    }

    public void Spawn(GameObject toSpawn)
    {
        Instantiate(toSpawn, gameObject.transform.position, Quaternion.Euler(new Vector3(0,Random.value*360,0)));
    }
}
