using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drawable : Interactable
{
    public int PageID;

    private void Start()
    {
        closeColor = new Vector3(1, 5, 5);
        hoverColor = new Vector3(1, 5, 2);
    }

    public override void Interact()
    {
        UIManager.AnnounceText("Use [J] to open your journal");
    }
}
