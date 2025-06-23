using System.Net;
using UnityEngine;

public class MineralClass
{
    
    public string Mineral = "BasicFuel";
    public GameObject MineralObject;
    public Fuel Fuel;
    public bool BeingMined = false;

    public MineralClass(string mineral, GameObject mineralObject, Fuel fuel)
    {
        this.Mineral = mineral;
        this.MineralObject = mineralObject;
        this.Fuel = fuel;
    }

}
