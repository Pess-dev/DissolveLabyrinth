using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [SerializeField]List<AudioClip> clips = new List<AudioClip>();
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomAt(){
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)]);
    }
}
