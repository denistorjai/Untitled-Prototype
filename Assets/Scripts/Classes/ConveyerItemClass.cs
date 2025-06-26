using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConveyerItem
{
    
    // Variables
    public string ObjectID;
    public GameObject ObjectPrefab;
    public string ItemName;
    public Fuel Fuel;
    public Vector2Int ItemGridPosition;
    public Vector2 HeadingDirection;
    public Tween CurrentTween;

}
