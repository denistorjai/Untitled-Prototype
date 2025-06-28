using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class ObjectClass
{
    
    // Variables
    
    public Vector3 ObjectPosition;
    public ItemClass ObjectItem;
    public Vector3 ObjectSize;
    public string ObjectID;
    public GameObject Object;
    public bool ActiveObject = false;
    public Vector2 OutputDirection = Vector2.right;
    public Vector2Int Gridpos;
    public Dictionary<string, ConveyerItem> ConveyerItems = new Dictionary<string, ConveyerItem>();
    public bool AllowConveyerItems = false;
    public float AllowedConveyerItems = 1;
    public string ObjectType;
    
    // Methods
    
    public void DeleteObject()
    {
        
        
    }
    
}
