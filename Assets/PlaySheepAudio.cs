using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySheepAudio : MonoBehaviour
{
    private AudioSource source;
    public AudioClip[] clips;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[Random.Range(0, clips.Length - 1)];
    }
    private void Start()
    {
        source.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        source.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
    }
}
