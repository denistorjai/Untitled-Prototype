using System.Collections.Generic;
using UnityEngine;

public class ItemClass
{
    
    // Variables
    
    public GameObject ObjectPrefab;
    public string ItemName;
    public string ItemType;
    public Sprite ItemIcon;
    public bool Unlocked;

    public ItemClass(GameObject objectPrefab, string itemName, string itemType, Sprite itemIcon, bool unlocked)
    {
        this.ObjectPrefab = objectPrefab;
        this.ItemName = itemName;
        this.ItemType = itemType;
        this.ItemIcon = itemIcon;
        this.Unlocked = unlocked;
    }

}
