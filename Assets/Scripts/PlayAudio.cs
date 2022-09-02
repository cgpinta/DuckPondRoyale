using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{
    public List<AudioClip> audioClips;
    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(string soundname)
    {
        foreach(AudioClip clip in audioClips)
        {
            if(clip.name == soundname)
            {
                audioSource.clip = clip;
                audioSource.Play(0);
            }
        }
    }
}
