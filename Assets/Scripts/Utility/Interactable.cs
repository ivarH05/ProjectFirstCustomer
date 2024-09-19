using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private const float glowDistance = 10;
    private bool outline = false;

    private bool hovering = false;
    public float outlineMultiplier = 1f;

    virtual internal void Update()
    {
        if ((transform.position - Player.Position).magnitude < glowDistance)
        {
            if (!outline)
                SetOutline(0.015f);
        }else if (outline && !hovering)
            SetOutline(0);
    }

    public void StartHover()
    {
        hovering = true;
        SetOutline(0.02f, 5f, 1f, 0f);
    }
    public void StopHover()
    {
        hovering = false;
        SetOutline(0);
    }

    public void SetOutline(float size, float r = 5, float g = 5, float b = 5)
    {
        outline = size > 0;
        if (!outline)
            size = -.0001f;
        size *= outlineMultiplier;

        List<MeshRenderer> meshes = GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();
        MeshRenderer ownMesh = GetComponent<MeshRenderer>();
        if (ownMesh != null)
            meshes.Add(ownMesh);
        for (int i = 0; i < meshes.Count; i++)
        {
            for (int j = 0; j < meshes[i].materials.Length; j++)
            {
                Material mat = meshes[i].materials[j];
                mat.SetFloat("_OutlineWidth", size);
                mat.SetColor("_OutlineColor", new Color(r, g, b));
            }
        }
    }
    public void DisableCollision()
    {
        List<Collider> colliders = GetComponentsInChildren<Collider>().ToList<Collider>();
        Collider ownCollider = GetComponent<Collider>();
        if (ownCollider != null)
            colliders.Add(ownCollider);
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = false;
        }
    }

    virtual public void OnHoverStart()
    {

    }
    virtual public void OnHoverStay()
    {

    }
    virtual public void OnHoverEnd()
    {

    }
    virtual public void Interact()
    {

    }
}
