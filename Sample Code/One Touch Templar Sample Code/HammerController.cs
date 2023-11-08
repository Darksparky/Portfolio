using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        int pState = player.GetComponent<PlayerController>().playerStateToInt();
        if ( pState == 0 || pState == 1)
        {
            gameObject.transform.position = player.gameObject.transform.position;
        }
    }



}
