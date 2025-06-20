using System.Collections.Generic;
using UnityEngine;

public class Fuel
{

    public string FuelName;
    public float FuelAmount;
    public GameObject ObjectPrefab;

    public Fuel(string fuelName, float fuelAmount, GameObject prefab)
    {
        this.FuelName = fuelName;
        this.FuelAmount = fuelAmount;
        this.ObjectPrefab = prefab;
    }
    
}
