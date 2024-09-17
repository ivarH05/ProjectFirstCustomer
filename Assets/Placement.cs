using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : Interactable
{
    public GameObject itemPrefab;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset = new Vector3(1, 1, 1);
    public int itemID;
    public override void Interact()
    {
        if (!Player.HasItem(itemID))
            return;
        GameObject obj = Instantiate(itemPrefab);
        obj.transform.position = transform.position + positionOffset;
        obj.transform.eulerAngles = transform.eulerAngles + rotationOffset;
        obj.transform.localScale = new Vector3(transform.localScale.x * scaleOffset.x, transform.localScale.y * scaleOffset.y, transform.localScale.z * scaleOffset.z);
        Destroy(gameObject);
    }
}
