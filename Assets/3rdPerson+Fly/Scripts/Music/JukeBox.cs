using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JukeBox : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> songs;
    private int previousSongIndex = -1;
    //Debug
    [Header("Debug juke box")]
    [Tooltip("This should be false unless testing in unity editor!!!!")]
    [SerializeField]
    private bool isDebug = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(WaitAndPlayNext());
    }

    private void Update()
    {
        if(isDebug && Input.GetKeyDown(KeyCode.P))
        {
            StopCurrentAndPlayNext();
        }
    }

    IEnumerator WaitAndPlayNext()
    {
        if(!isDebug)
        {
            int randomSong = Random.Range(0, songs.Count);
            if (previousSongIndex == randomSong)
            {
                randomSong = WrapIndex(randomSong);
            }
            audioSource.clip = songs[randomSong];
            audioSource.Play();
            yield return new WaitForSeconds(songs[randomSong].length);
            previousSongIndex = randomSong;
            StartCoroutine(WaitAndPlayNext());
        }
        
    }
    
    void StopCurrentAndPlayNext()
    {
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        int randomSong = Random.Range(0, songs.Count);
        if (previousSongIndex == randomSong)
        {
            randomSong = WrapIndex(randomSong);
        }
        audioSource.clip = songs[randomSong];
        audioSource.Play();
        previousSongIndex = randomSong;
    }

    private int WrapIndex(int index)
    {
        int next = index + 1;
        return Mathf.Clamp(next, 0, songs.Count-1);
    }
}
