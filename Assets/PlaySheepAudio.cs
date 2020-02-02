using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySheepAudio : MonoBehaviour
{
    private AudioSource source;
    public AudioClip[] clips;
    int rng;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[Random.Range(0, clips.Length - 1)];
    }
    private void Start()
    {
        source.PlayOneShot(clips[Random.Range(0, clips.Length - 1)], 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rng = Random.Range(0, 10);
        if (rng >= 9)
        {
            source.PlayOneShot(clips[Random.Range(0, clips.Length - 1)], 1f);
        }
        
    }
}
