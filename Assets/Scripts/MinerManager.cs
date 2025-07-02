using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class MinerManager : MonoBehaviour
{
    
    public static MinerManager Instance;
    
    // Round Multiplier
    public float RoundMultiplier;
    
    // Variable
    
    Dictionary<string, MineralClass> Minerals = new Dictionary<string, MineralClass>();
    Dictionary<string, Fuel> Fuels = new Dictionary<string, Fuel>();
    public GameObject[] FuelPrefabs;
    public GameObject[] MineralPrefabs;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Game Fuels
        
        Fuel BasicFuel = new Fuel("BasicFuel", 5, FuelPrefabs[0]);
        Fuels.Add(BasicFuel.FuelName, BasicFuel);
        
    }

    public void StartRound()
    {
        PlaceMinerals();
    }
    
    public void PlaceMinerals()
    {
        RoundMultiplier = PlayerManager.Instance.ReturnRoundMultiplier();
        float NumberofMinerals = UnityEngine.Random.Range(3, 5);

        for (int i = 0; i < NumberofMinerals; i++)
        {
            // X & Y Randomization
            float XPosition = UnityEngine.Random.Range(-15, 15);
            float YPosition = UnityEngine.Random.Range(-15, 15);
            while (XPosition == 0 && YPosition == 0 || XPosition == 0 || YPosition == 0)
            {
                XPosition = UnityEngine.Random.Range(-15, 15);
                YPosition = UnityEngine.Random.Range(-15, 15);
            }
            Vector3 Position = new Vector3(XPosition, YPosition, 0);
            MineralClass Mineral = new MineralClass("BasicFuel", Instantiate(MineralPrefabs[0], Position, UnityEngine.Quaternion.identity), Fuels["BasicFuel"]);
            Minerals.Add(ReturnID(), Mineral);
        }
        
    }
    
    string ReturnID()
    {
        string Characters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var ID = "";
        for (int i = 0; i < 9; i++) {
            ID += Characters[UnityEngine.Random.Range(0, Characters.Length)];
        }
        return ID;
    }
    
    public Dictionary<string, MineralClass> GetMinerals()
    {
        return Minerals;
    }
    
}
