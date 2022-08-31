using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;

    public bool play;


    // Start is called before the first frame update
    void Start()
    {
        play = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    // Update is called once per frame
    void Update()
    {
        if(play)
        {
            audioSource.Play(0);
            play = false;
        }
    }
}
