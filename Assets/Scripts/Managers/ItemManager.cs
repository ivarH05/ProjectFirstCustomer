using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ItemManager : MonoBehaviour
{
    private static ItemManager singleton;
    public Item[] items;

    private void Start()
    {
        singleton = this;
    }

    private void Update()
    {
        if (!Application.isEditor)
            return;
        for (int i = 0; i < items.Length; i++)
        {
            items[i].id = i;
        }
    }

    public static Item GetItem(int index)
    { 
        return singleton.items[index]; 
    }
}
