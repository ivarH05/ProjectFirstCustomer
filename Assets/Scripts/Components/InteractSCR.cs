using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSCR : MonoBehaviour
{
    [SerializeField] private List<int> inventory;
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
        itemObject.transform.position = Vector3.Lerp(itemObject.transform.position, cam.transform.position - new Vector3(0, 0.1f, 0), Time.deltaTime * 15);
        itemObject.transform.localScale = Vector3.Lerp(itemObject.transform.localScale, Vector3.zero, Time.deltaTime * 25);
        if (Vector3.Distance(itemObject.transform.position, player.transform.position) < 0.15f)
        {
            Destroy(itemObject);
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
        inventory.Remove(id);
        return true;
    }

    public void PickupItem(GameObject GO, int itemIndex)
    {
        if (itemObject != null)
            Destroy(itemObject);
        itemObject = GO;
        inventory.Add(itemIndex);
        UIManager.AnnounceItem(itemIndex);
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
