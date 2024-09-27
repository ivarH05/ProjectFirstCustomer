using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSCR : Interactable
{
    public int itemIndex;

    public bool used = false;
    public override void Interact()
    {
        Player.interact.PickupItem(gameObject, itemIndex);
        SetOutline(0);
        DisableCollision();
        used = true;
    }
}
