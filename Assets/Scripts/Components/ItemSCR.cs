using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSCR : Interactable
{
    public int itemIndex;
    public override void Interact()
    {
        Player.interact.PickupItem(itemIndex, gameObject);
        SetOutline(0);
        DisableCollision();
    }

    public virtual void OnPickup()
    {

    }

    public virtual void OnCompletedPickup()
    {
        Destroy(gameObject);
    }

    public virtual void OnUse()
    {

    }
}
