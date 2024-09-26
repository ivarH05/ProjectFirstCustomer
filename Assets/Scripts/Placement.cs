using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Placement : Interactable
{
    public bool dissableAfter = true;
    public GameObject itemPrefab;
    public int itemID;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset = new Vector3(1, 1, 1);

    Vector3 targetLocation;
    Vector3 targetScale;
    public bool isPlaced = false;
    GameObject obj;
    private void Start()
    {
        targetLocation = transform.position + positionOffset;
        targetScale = new Vector3(transform.localScale.x * scaleOffset.x, transform.localScale.y * scaleOffset.y, transform.localScale.z * scaleOffset.z);

        obj = Instantiate(itemPrefab);
        obj.transform.position = transform.position + positionOffset;
        obj.transform.eulerAngles = transform.eulerAngles + rotationOffset;
        obj.transform.localScale = new Vector3(transform.localScale.x * scaleOffset.x, transform.localScale.y * scaleOffset.y, transform.localScale.z * scaleOffset.z);

        List<Renderer> renderers = obj.transform.GetComponentsInChildren<Renderer>().ToList();
        Material mat = ItemManager.getPreviewMaterial();
        renderers.Add(obj.GetComponent<Renderer>());
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].SetMaterials(new List<Material> { mat });
        }

        List<Collider> colliders = obj.transform.GetComponentsInChildren<Collider>().ToList();
        for (int i = 0; i < colliders.Count; i++)
        {
            Destroy(colliders[i]);
        }
    }

    private void Update()
    {
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
        Destroy(obj);

        obj = Instantiate(itemPrefab);

        obj.transform.position = Player.camera.transform.position - Player.camera.transform.up * 0.1f;

        obj.transform.eulerAngles = transform.eulerAngles + rotationOffset;

        obj.transform.localScale = new Vector3(0, 0, 0);

        isPlaced = true;
        if(dissableAfter)
            GetComponent<Collider>().enabled = false;
    }
}
