using System.Collections.Generic;
using UnityEngine;

public class MinerManager : MonoBehaviour
{
    
    // Variable
    
    Dictionary<string, MineralClass> Minerals = new Dictionary<string, MineralClass>();
    Dictionary<string, Fuel> Fuels = new Dictionary<string, Fuel>();
    public GameObject[] FuelPrefabs;
    
    // Temporary
    public GameObject[] MinerObjects;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Fuel BasicFuel = new Fuel("BasicFuel", 5, FuelPrefabs[0]);
        Fuels.Add(BasicFuel.FuelName, BasicFuel);
        
        // Temporary, Make Minerals
        MineralClass Mineral1 = new MineralClass("BasicFuel", MinerObjects[0], BasicFuel);
        MineralClass Mineral2 = new MineralClass("BasicFuel", MinerObjects[1], BasicFuel);
        MineralClass Mineral3 = new MineralClass("BasicFuel", MinerObjects[2], BasicFuel);
        Minerals.Add("Mineral1", Mineral1);
        Minerals.Add("Mineral2", Mineral2);
        Minerals.Add("Mineral3", Mineral3);
    }

    public Dictionary<string, MineralClass> GetMinerals()
    {
        return Minerals;
    }
    
}
