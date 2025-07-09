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
    public string ItemDescription;
    public string ItemUpgradeName;

    public ItemClass(GameObject objectPrefab, string itemName, string itemType, Sprite itemIcon, bool unlocked, string itemDescription, string itemUpgradeName)
    {
        this.ObjectPrefab = objectPrefab;
        this.ItemName = itemName;
        this.ItemType = itemType;
        this.ItemIcon = itemIcon;
        this.Unlocked = unlocked;
        this.ItemDescription = itemDescription;
        this.ItemUpgradeName = itemUpgradeName;
    }

}
