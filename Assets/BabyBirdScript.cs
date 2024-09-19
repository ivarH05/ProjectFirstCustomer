using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BabyBirdScript : ItemSCR
{
    AudioSource source;
    public AudioClip birdPanic;
    public AudioClip birdCommands;

    private bool pickedUp = false;
    private bool done = false;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = birdPanic;
        source.Play();
    }
    internal override void Update()
    {
        base.Update();
        if (!pickedUp)
            return;

        float dist = Vector3.Distance(GameManager.getBirdNest().transform.position, Player.Position);
        source.volume = 1 - (4 / (dist + 4));
        source.spatialBlend = 0;
    }

    public override void OnPickup()
    {
        Debug.Log("Pickup");
        pickedUp = true;
        done = false;
    }
    public override void OnCompletedPickup()
    {
        Debug.Log("DonePickup");
    }
    public override void OnUse()
    {
        Debug.Log("Used");
        done = true;
    }
}
