using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    private static Dictionary<string, AudioClip[]> dict = new Dictionary<string, AudioClip[]>();
    public static void Initialize()
    {
        dict = new Dictionary<string, AudioClip[]>()
        {
            {
                "PlayerFootstepGrass",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("FootstepGrass1")
                } 
            }
        };
    }

    public static void PlayOneShot(string clipID, AudioSource source, float volume = 1, float animalRange = 0, float pitchShift = 0.1f)
    {
        AudioClip[] clips = dict[clipID];
        if (clips == null)
            return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null)
        {
            Debug.LogError("incorrect path to clip");
            return;
        }

        source.pitch = 1 - Random.Range(-pitchShift, pitchShift);
        source.volume = volume;
        source.PlayOneShot(clip);
        if (animalRange > 0)
            AnimalManager.PlaySound(source.transform.position, animalRange);
    }
}
