using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MusicMix
{
    [Range(0f, 2f)]
    public float Flute = 1;
    [Range(0f, 2f)]
    public float HandPan = 1;
    [Range(0f, 2f)]
    public float Harp = 1;
    [Range(0f, 2f)]
    public float Gong = 1;
    [Range(0f, 2f)]
    public float Synth = 1;

    public float GetVolumeByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return Flute;
            case 1:
                return HandPan;
            case 2:
                return Harp;
            case 3:
                return Gong;
            case 4:
                return Synth;
            default:
                return 1;
        }
    }
}

public class MusicManager : MonoBehaviour
{
    public AudioClip[] instruments;

    public MusicMix standard;

    public MusicMix[] mixes;
    public bool[] activity;
    private AudioSource[] sources;

    // Start is called before the first frame update
    void Start()
    {
        activity = new bool[instruments.Length];
        sources = new AudioSource[instruments.Length];
        for (int i = 0; i < instruments.Length; i++)
        {
            AudioSource s = transform.AddComponent<AudioSource>();
            s.clip = instruments[i];
            s.volume = standard.GetVolumeByIndex(i);
            s.loop = true;
            s.priority = 1;
            s.Play();
            sources[i] = s;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < instruments.Length; i++)
        {
            float volume = 1;
            volume *= standard.GetVolumeByIndex(i);
            for (int j = 0; j < mixes.Length; j++)
            {
                if (activity[j] == false)
                    continue;
                volume *= mixes[j].GetVolumeByIndex(i);
            }
            sources[i].volume = Mathf.Lerp(sources[i].volume, volume, Time.deltaTime);
        }
    }
}
