using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    
    // Variables
    
    public GridPlacement gridPlacement;
    public UIHandler UIhandler;
    public float CurrentFuel = 0;
    public float CurrentTimeLeft = 10;
    public float MinimumScore = 500;
    
    // Placing

    private bool CurrentlyPlacing = false;
    
    public void ItemPlacingPreview(ItemClass item)
    {
        if (CurrentlyPlacing)
        {
            gridPlacement.removePreview();
            gridPlacement.setPreview(item);
        }
        else
        {
            gridPlacement.setPreview(item);
        }
        CurrentlyPlacing = true;
        UIhandler.ToggleToopTip(CurrentlyPlacing);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Placement
        
        if (CurrentlyPlacing)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                if (EventSystem.current.currentSelectedGameObject)
                {
                    if (EventSystem.current.currentSelectedGameObject.CompareTag("UI"))
                    {
                        gridPlacement.PlaceObject();
                    }
                }
                else
                {
                    gridPlacement.PlaceObject();
                }
            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                gridPlacement.rotatePreview();
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                gridPlacement.removePreview();
                CurrentlyPlacing = false;
                UIhandler.ToggleToopTip(CurrentlyPlacing);
            }

        }
        
        // Round Timer
        CurrentTimeLeft -= Time.deltaTime;

        if (CurrentTimeLeft <= 0)
        {
            if (CurrentFuel < MinimumScore)
            {
                GameManager.instance.EndGame();
            }
        }
        
        UIhandler.UpdateScoreUI(CurrentTimeLeft, CurrentFuel, MinimumScore);
        
    }
    
    // Methods
    public void AddFuel(float FueltoAdd)
    {
        CurrentFuel += FueltoAdd;
    }
    
}
