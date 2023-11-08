using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHammer : MonoBehaviour
{
    GameObject hammer;
    // Start is called before the first frame update
    void Start()
    {
        hammer = GameObject.FindGameObjectWithTag("Hammer");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position = hammer.transform.position;
    }
}
