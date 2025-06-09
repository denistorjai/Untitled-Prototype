using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    
    // Variables
    
    public GridPlacement gridPlacement;
    
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
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentlyPlacing)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                gridPlacement.PlaceObject();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                gridPlacement.rotatePreview();
            }
        }
    }

    // Methods

    
}
