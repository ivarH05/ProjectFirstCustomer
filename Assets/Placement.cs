using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : Interactable
{
    public GameObject itemPrefab;
    public int itemID;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset = new Vector3(1, 1, 1);

    Vector3 targetLocation;
    Vector3 targetScale;
    bool isPlaced = false;
    GameObject obj;

    override internal void Update()
    {
        base.Update();
        if (obj == null)
            return;
        Vector3 target = Vector3.Lerp(
            targetLocation,
            targetLocation + new Vector3(0, 0.5f, 0),
            Mathf.Clamp(Vector3.Distance(obj.transform.position, targetLocation) / 2, 0, 0.999f)
            );
        obj.transform.position = Vector3.Lerp(obj.transform.position, target, Time.deltaTime * 15);
        obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, targetScale, Time.deltaTime * 15);
    }

    public override void Interact()
    {
        if (isPlaced)
        {
            Player.PickupItem(itemID, obj);
            obj = null;
            isPlaced = false;
            return;
        }

        if (!Player.UseItem(itemID))
            return;
        obj = Instantiate(itemPrefab);

        obj.transform.position = Player.camera.transform.position - Player.camera.transform.up * 0.1f;
        targetLocation = transform.position + positionOffset;

        obj.transform.eulerAngles = transform.eulerAngles + rotationOffset;

        targetScale = new Vector3(obj.transform.localScale.x * scaleOffset.x, obj.transform.localScale.y * scaleOffset.y, obj.transform.localScale.z * scaleOffset.z);

        obj.transform.localScale = new Vector3(0, 0, 0);
        isPlaced = true;
    }
}
