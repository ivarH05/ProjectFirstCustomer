using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager singleton;
    public ItemAnnouncer itemAnnouncer;

    private void Start()
    {
        singleton = this;
    }

    public static void AnnounceItem(int itemIndex)
    {
        ItemAnnouncer ia = singleton.itemAnnouncer;
        ia.animate(ItemManager.GetItem(itemIndex).name);
    }
}
