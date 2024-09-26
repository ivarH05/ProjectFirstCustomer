using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSCR : MonoBehaviour
{
    [Header("Setup")]
    public Animator journal;
    public MeshRenderer journalLeft;
    public MeshRenderer journalPage;
    public MeshRenderer journalRight;
    public List<Texture2D> journalPages = new List<Texture2D>();
    public int pageIndex;

    [Header("Settings")]
    public Texture2D[] AllPages;

    [Header("Data")]
    [SerializeField] private List<int> inventory;
    public float maxDistance = 1.5f;


    private GameObject itemObject;

    Interactable currentHover;
    PlayerController player;
    Camera cam;

    private bool usingJournal;

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
        if (Input.GetKeyDown(KeyMapping.Journal) || (Input.GetKeyDown(KeyMapping.Escape) && usingJournal))
        {
            SetJournalActivity(!usingJournal);
        }
        if (!usingJournal)
        {
            CheckInteract();
            HandleCurrentItem();
            return;
        }
        if (Input.GetKeyDown(KeyMapping.NextPage))
        {
            int nextIndex = pageIndex + 1;
            if (nextIndex >= journalPages.Count)
                return;
            journalLeft.materials[0].SetTexture("_BaseColorMap", journalPages[pageIndex]);
            journalPage.materials[0].SetTexture("_BaseColorMap", journalPages[pageIndex]);
            journalPage.materials[1].SetTexture("_BaseColorMap", journalPages[nextIndex]);
            journalRight.materials[0].SetTexture("_BaseColorMap", journalPages[nextIndex]);
            journal.SetTrigger("NextPage");
            pageIndex = nextIndex;
        }

        if (Input.GetKeyDown(KeyMapping.PreviousPage))
        {
            int nextIndex = pageIndex - 1;
            if (nextIndex < 0)
                return;
            journalLeft.materials[0].SetTexture("_BaseColorMap", journalPages[nextIndex]);
            journalPage.materials[0].SetTexture("_BaseColorMap", journalPages[nextIndex]);
            journalPage.materials[1].SetTexture("_BaseColorMap", journalPages[pageIndex]);
            journalRight.materials[0].SetTexture("_BaseColorMap", journalPages[pageIndex]);
            journal.SetTrigger("PreviousPage");
            pageIndex = nextIndex;
        }
    }

    public void SetJournalActivity(bool open)
    {
        usingJournal = open;
        journal.SetBool("Open", usingJournal);
        Player.canMove = !usingJournal;
        CameraController.canMove = !usingJournal;
        CameraController.targetXRotation = 45;
        Player.proceduralAnimator.StartRotating();

        journalLeft.materials[0].SetTexture("_BaseColorMap", journalPages[pageIndex]);
        journalPage.materials[0].SetTexture("_BaseColorMap", journalPages[pageIndex]);
        journalPage.materials[1].SetTexture("_BaseColorMap", journalPages[pageIndex]);
        journalRight.materials[0].SetTexture("_BaseColorMap", journalPages[pageIndex]);
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
        GiveItem(itemIndex);
    }

    public void GiveItem(int id)
    {
        inventory.Add(id);
        UIManager.AnnounceItem(id);
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
            {
                i.Interact();
                if (i is Drawable d && !journalPages.Contains(AllPages[d.PageID]))
                {
                    pageIndex = journalPages.Count - 1;
                    journalPages.Insert(journalPages.Count - 1, AllPages[d.PageID]);
                    SetJournalActivity(true);
                    journalLeft.materials[0].SetFloat("_StartTime", Time.time);
                    journalPage.materials[0].SetFloat("_StartTime", Time.time);
                    journalPage.materials[1].SetFloat("_StartTime", Time.time);
                    journalRight.materials[0].SetFloat("_StartTime", Time.time);
                }
            }
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
        if (interactable == null)
            return hit.transform.GetComponentInParent<Interactable>();
        return interactable;
    }
}
