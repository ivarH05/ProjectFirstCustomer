using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMixer : MonoBehaviour
{
    public float radius = 10;
    public int mixIndex;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, Player.Position) < radius)
        {
            MusicManager.SwitchActive(mixIndex);
        }
    }
}
