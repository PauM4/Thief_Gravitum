using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource myAudioSource = GetComponent<AudioSource>();
        myAudioSource.time = Random.Range(0.0f, myAudioSource.clip.length);
        myAudioSource.Play();
    }
}
