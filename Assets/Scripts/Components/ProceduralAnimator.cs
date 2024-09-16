using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    [Header("Setup")]
    public PlayerController player;

    bool rotating = false;
    Quaternion targetQuaternion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player.velocity.magnitude > 10)
        {
            targetQuaternion = Quaternion.Euler(0, player.cam.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 10);
        }
        else
        {
            float difference = Mathf.Abs(player.cam.transform.eulerAngles.y - transform.eulerAngles.y);
            if (difference > 45)
                rotating = true;
            if (difference < 15)
                rotating = false;

            if(rotating)
                targetQuaternion = Quaternion.Euler(0, player.cam.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 2);
        }
    }
}
