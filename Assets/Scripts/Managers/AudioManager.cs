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
                "PlayerFootstepDirt",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("DirtStep1"),
                    (AudioClip)Resources.Load("DirtStep2"),
                    (AudioClip)Resources.Load("DirtStep3")
                }
            },
            {
                "PlayerFootstepGrass",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("GrassStep1"),
                    (AudioClip)Resources.Load("GrassStep2"),
                    (AudioClip)Resources.Load("GrassStep3")
                }
            },
            {
                "PlayerFootstepGravel",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("GravelStep1"),
                    (AudioClip)Resources.Load("GravelStep2"),
                    (AudioClip)Resources.Load("GravelStep3")
                }
            },
            {
                "PlayerFootstepIce",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("IceStep1"),
                    (AudioClip)Resources.Load("IceStep2"),
                    (AudioClip)Resources.Load("IceStep3")
                }
            },
            {
                "PlayerFootstepRock",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("GravelStep1"),
                    (AudioClip)Resources.Load("GravelStep2"),
                    (AudioClip)Resources.Load("GravelStep3")
                }
            },
            {
                "PlayerFootstepWood",
                new AudioClip[]
                {
                    (AudioClip)Resources.Load("WoodStep1"),
                    (AudioClip)Resources.Load("WoodStep2"),
                    (AudioClip)Resources.Load("WoodStep3")
                }
            },
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
