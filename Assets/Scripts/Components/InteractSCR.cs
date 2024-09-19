using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSCR : MonoBehaviour
{
    [SerializeField] private List<int> inventory = new List<int>();
    [SerializeField] private List<GameObject> inventoryObjects = new List<GameObject>();
    public float maxDistance = 1.5f;

    PlayerController player;
    Camera cam;

    Interactable currentHover;

    private GameObject itemObject;

    // Start is called before the first frame update
    void Start()
    {
        Player.interact = this;
        player = GetComponent<PlayerController>();
        cam = player.cam;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteract();
        HandleCurrentItem();

    }

    private void HandleCurrentItem()
    {
        if (itemObject == null)
            return;
        Vector3 targetPos = cam.transform.position - cam.transform.up * 0.1f;
        itemObject.transform.position = Vector3.Lerp(itemObject.transform.position, targetPos, Time.deltaTime * 15);
        itemObject.transform.localScale = Vector3.Lerp(itemObject.transform.localScale, Vector3.zero, Time.deltaTime * 25);
        if (Vector3.Distance(itemObject.transform.position, targetPos) < 0.1f)
        {
            itemObject.GetComponent<ItemSCR>().OnCompletedPickup();
            itemObject = null;
        }
    }

    public bool HasItem(int id)
    {
        return inventory.Contains(id);
    }
    public bool UseItem(int id)
    {
        if(!inventory.Contains(id)) 
            return false;
        int index = inventory.IndexOf(id);
        if(inventoryObjects[index] != null)
            inventoryObjects[index].GetComponent<ItemSCR>().OnUse();
        inventory.RemoveAt(index);
        inventoryObjects.RemoveAt(index);
        return true;
    }

    public void PickupItem(int itemIndex, GameObject GO)
    {
        if (itemObject != null)
            itemObject.GetComponent<ItemSCR>().OnCompletedPickup();
        itemObject = GO;
        itemObject.GetComponent<ItemSCR>().OnPickup();
        GiveItem(itemIndex, GO);
    }

    public void GiveItem(int id, GameObject GO = null)
    {
        inventory.Add(id);
        UIManager.AnnounceItem(id);
        inventoryObjects.Add(GO);
    }

    private void CheckInteract()
    {
        Interactable i = GetHoveringInteractable();
        if (i == currentHover)
        {
            if (currentHover == null)
                return;
            i.OnHoverStay();
            if (Input.GetKeyDown(KeyMapping.Interact))
                i.Interact();
        }
        else
        {
            if (currentHover != null)
                currentHover.StopHover();
            if (i != null)
                i.StartHover();
        }
        currentHover = i;
    }

    private Interactable GetHoveringInteractable()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistance, ~(1 << 6)))
            return null;


        Interactable interactable = hit.transform.GetComponent<Interactable>();
        if(interactable == null)
            return hit.transform.GetComponentInParent<Interactable>();
        return interactable;
    }
}
