using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    
    public static PlayerManager Instance;
    
    // Variables
    
    public List<ItemClass> items;
    public List<ItemClass> availableupgrades;
    public List<ItemClass> availablerareupgrades;
    public List<ItemClass> availablelegendaryupgrades;
    
    public float CurrentFuel = 0;
    private float RoundMultiplier = 1;
    private float CurrentTimeLeft = 60;
    private float StartTime = 60;
    private float MinimumScore = 50;
    private bool RoundActive = false;
    public float MouseSens = 0.03f;
    private bool InUpgradeProgress = false;
    
    private static Vector3 ClampMax = new Vector3(13, 13, -1);
    private static Vector3 ClampMin = new Vector3(-13, -13, -1);
    
    // Items
    public Sprite[] sprites;
    public GameObject[] prefabs;
    
    // Placing

    private bool CurrentlyPlacing = false;
    
    public void ItemPlacingPreview(ItemClass item)
    {
        if (CurrentlyPlacing)
        {
            GridPlacement.Instance.removePreview();
            GridPlacement.Instance.setPreview(item);
        }
        else
        {
            GridPlacement.Instance.setPreview(item);
        }
        CurrentlyPlacing = true;
        UIHandler.Instance.ToggleToopTip(CurrentlyPlacing);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
        
        // Starting Items
        items = new List<ItemClass>()
        {
            new ItemClass(prefabs[0], "Conveyer", "Conveyer", sprites[0], true, "Conveyer Belt", "Basic Conveyer Belt"),
            new ItemClass(prefabs[1], "Miner", "Miner", sprites[1], true, "Mineral Miner", "Basic Mineral Miner")
        };
        
        RoundStart();
        
        // Upgrades
        
        // Common Upgrades
        availableupgrades = new List<ItemClass>()
        {
            new ItemClass(prefabs[1], "Miner", "Miner", sprites[1], true, "A newly engineered miner that lets you mine minerals at 25% increased speed", "Miner Upgrade+"),
            new ItemClass(prefabs[0], "Conveyer", "Conveyer", sprites[0], true, "A basic conveyer belt that was overclocked to allow for faster transporation of items", "Conveyer Upgrade+"),
            new ItemClass(prefabs[1], "Miner", "Refiner", sprites[1], true, "An rich mineral processor that allows ore to be refined for more value", "Ore Refinement+")
        };

    }
    
    public bool MouseHeld = false;
    
    // Update is called once per frame
    void Update()
    {
        
        // Round Timer
        
        CurrentTimeLeft -= Time.deltaTime;
        
        if (RoundActive == true)
        {
            
            // Camera Controls

            if (Keyboard.current.wKey.isPressed)
            {
                CameraHandler.Instance.gameObject.transform.Translate(Vector3.up * 0.1f);
            }
            
            if (Keyboard.current.sKey.isPressed)
            {
                CameraHandler.Instance.gameObject.transform.Translate(Vector3.down * 0.1f);
            }
            
            if (Keyboard.current.dKey.isPressed)
            {
                CameraHandler.Instance.gameObject.transform.Translate(Vector3.right * 0.1f);
            }
            
            if (Keyboard.current.aKey.isPressed)
            {
                CameraHandler.Instance.gameObject.transform.Translate(Vector3.left * 0.1f);
            }

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                CameraHandler.Instance.gameObject.transform.Translate(new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * MouseSens);
            }
 
            Vector3 ClampedCameraPosition = new Vector3(
                Mathf.Clamp(CameraHandler.Instance.gameObject.transform.position.x, ClampMin.x, ClampMax.x ),
                Mathf.Clamp(CameraHandler.Instance.gameObject.transform.position.y, ClampMin.y, ClampMax.y),
                -1
            );
            
            CameraHandler.Instance.gameObject.transform.position = ClampedCameraPosition;
            
            // Placement
            
            if (CurrentlyPlacing)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    if (EventSystem.current.currentSelectedGameObject)
                    {
                        if (EventSystem.current.currentSelectedGameObject.CompareTag("UI"))
                        {
                            GridPlacement.Instance.PlaceObject();
                        }
                    }
                    else
                    {
                        GridPlacement.Instance.PlaceObject();
                    }
                }
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    GridPlacement.Instance.rotatePreview();
                }
                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    GridPlacement.Instance.removePreview();
                    CurrentlyPlacing = false;
                    UIHandler.Instance.ToggleToopTip(CurrentlyPlacing);
                }
            }
            
            if (CurrentTimeLeft <= 0)
            {
                if (CurrentFuel < MinimumScore)
                {
                    EndRound();
                }
            }
    
            if (CurrentFuel >= MinimumScore)
            {
                CurrentlyPlacing = false;
                RoundActive = false;
                GameManager.instance.NextRound();
            }
        
            UIHandler.Instance.UpdateScoreUI(CurrentTimeLeft, CurrentFuel, MinimumScore);
        }
        
    }

    public void SetUpgrades()
    {
        // Items to Send
        List<ItemClass> upgrades = new List<ItemClass>();
        
        while (upgrades.Count < 3)
        {
            var Upgrade = availableupgrades[UnityEngine.Random.Range(0, availableupgrades.Count)];
            if (upgrades.Contains(Upgrade) == false)
            {
                upgrades.Add(Upgrade);
            }
        }
        
        UpgradeUIhandler.Instance.SetUpgrades(upgrades, RoundMultiplier);
    }

    public void AllowUpgrades(ItemClass upgrade)
    {
        items.Add(upgrade);
    }
    
    public void RoundStart()
    {
        CurrentTimeLeft = StartTime;
        CurrentFuel = 0;
        MinerManager.Instance.StartRound();
        GridPlacement.Instance.StartRound();
        UIHandler.Instance.StartRound(items);
        RoundActive = true;
    }
    
    public void StartGame()
    {
        RoundMultiplier = 1;
        StartTime = 10;
        RoundStart();
    }
    
    public void EndRound()
    {
        CurrentlyPlacing = false;
        RoundActive = false;
        GameManager.instance.EndGame();
    }

    public void StartNextRound()
    {
        RoundMultiplier = RoundMultiplier + 1;
        MinimumScore = MinimumScore * RoundMultiplier;
        StartTime = StartTime * RoundMultiplier;
        GameManager.instance.StartNextRound();
    }
    
    // Methods
    public void AddFuel(float FueltoAdd)
    {
        CurrentFuel += FueltoAdd;
    }

    public float ReturnRoundMultiplier()
    {
        return RoundMultiplier;
    }
    
}
