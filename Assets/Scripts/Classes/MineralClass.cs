using System.Net;
using UnityEngine;

public class MineralClass
{
    
    public string Mineral = "BasicFuel";
    public Vector2Int GridPos;
    public GameObject MineralObject;
    public Fuel Fuel;
    public bool BeingMined = false;

    public MineralClass(string mineral, GameObject mineralObject, Fuel fuel)
    {
        this.Mineral = mineral;
        this.MineralObject = mineralObject;
        this.Fuel = fuel;
        this.GridPos = new Vector2Int(  Mathf.RoundToInt(mineralObject.transform.position.x), Mathf.RoundToInt(mineralObject.transform.position.y) );
    }

}
