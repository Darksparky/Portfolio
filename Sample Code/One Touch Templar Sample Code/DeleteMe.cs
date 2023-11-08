using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMe : MonoBehaviour
{
    public float LifeTime;
    // Start is called before the first frame update
    void Start()
    {
        if (LifeTime != 0)
        {
            Destroy(gameObject, LifeTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void delete()
    {
        Destroy(gameObject);
        gameObject.SetActive(false);

    }
}
