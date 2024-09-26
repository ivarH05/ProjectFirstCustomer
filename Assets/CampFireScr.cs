using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampFireScr : MonoBehaviour
{
    List<Placement> placables;
    public bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        placables = transform.GetComponentsInChildren<Placement>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < placables.Count; i++)
        {
            if (!placables[i].isPlaced)
                return;
        }
        done = true;
        this.enabled = false;
    }
}
