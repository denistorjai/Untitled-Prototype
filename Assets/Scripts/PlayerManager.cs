using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    
    // Variables
    public GridPlacement gridPlacement;
    
    private Boolean CurrentlyPlacing = false;
    private GameObject PlacingItem;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (CurrentlyPlacing)
            {
                gridPlacement.placeTile(PlacingItem);
            }
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            gridPlacement.RotateTile();
        }
    }

    public void StartPlacing(GameObject Tile) {
        if (CurrentlyPlacing == true) {
            StopPlacing();
        } 
        PlacingItem = Tile;
        gridPlacement.setPreview(Tile);
        CurrentlyPlacing = true;
    }

    void StopPlacing() {
        gridPlacement.removePreview();
    }
    
}
