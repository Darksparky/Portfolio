using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class FootSteps : MonoBehaviour
{
    // Start is called before the first frame update
    NavMeshAgent a;
    public float minVel = 0;
    
    public AudioSource source;  
    public List<AudioClip> clips = new List<AudioClip>();

    void Start()
    {
        a = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if ( a.velocity.magnitude > minVel && source.isPlaying == false)
        {
            int maxRange = clips.Count - 1;
            int choice = Mathf.RoundToInt(Random.value * maxRange);
            source.clip = clips[choice];
            source.volume = Random.Range(0.6f, 1);
            source.pitch = Random.Range(0.8f, 1.1f);
            source.Play();
        }
    }
}