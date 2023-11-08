using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundList : MonoBehaviour
{
    public AudioSource source;
    public List<AudioClip> clips = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Randomizes the sfx that is attatched to the audiosource
    /// </summary>
    public void Randomize()
    {
        int maxRange = clips.Count - 1;
        int choice = Mathf.RoundToInt(Random.value * maxRange);
        source.clip = clips[choice];
    }
    public void Play()
    {
        source.Play();
    }
}
