using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    Dictionary<string, AudioClip> sfx = new Dictionary<string, AudioClip>();
    public List<AudioClip> audioClips;

    AudioSource audioSource;

    void Awake()
    {
        foreach(AudioClip ac in audioClips)
        {
            sfx.Add(ac.name,ac);
        }
        audioSource = GetComponent<AudioSource>();
    }
    public void playSound(string name)
    {
        audioSource.clip = sfx[name];
        audioSource.Play();
    }
}
