using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionSCR : ItemSCR
{
    public GameObject lion;
    public GameObject lionAgent;
    public ItemSCR Net;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Net.used)
            gameObject.SetActive(false);
        if (used)
        {
            lionAgent.SetActive(true);
            Destroy(gameObject);
        }
    }
}
