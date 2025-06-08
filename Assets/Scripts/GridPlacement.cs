using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class GridPlacement : MonoBehaviour
{
    // Variables
    public float gridSize;
    public Camera cam;

    private GameObject GhostObject;
    private Boolean DisplayingGhost = false;
    private int GhostRotation = 1;

    public Sprite[] sprites;
    
    // List of Current Tiles
    Dictionary<string, ObjectClass> TilePositions = new Dictionary<string, ObjectClass>();

    // Update is called once per frame
    void Update() {
        // Read Cursor Position, Add Mouse Offset, Set Ghost Object & Clamp to Grid
        if (DisplayingGhost != false) {
            var ObjectWorldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            GhostObject.transform.position = ClamptoGrid(gridSize, ObjectWorldPos);
        }
    }

    // Return Grid, / gridsize to turn (ex. 55,56,57) to similar numbers, and then * by gridsize to return them to place.
    Vector2 ClamptoGrid(float gridsize, Vector2 currentpos) { 
        return new Vector3(
            Mathf.Floor(currentpos.x / gridsize) * gridsize, 
            Mathf.Floor(currentpos.y / gridsize) * gridsize, 
            0);
    }

    string ReturnPrefabID()
    {
        string Characters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string GeneratedID = "";
        var x = 0;
        while (x < 9) {
            GeneratedID += Characters[UnityEngine.Random.Range(0, Characters.Length)];
            x += 1;
        }
        return GeneratedID;
    }
    
    // Display Placing Tile
    public void setPreview(GameObject PlacePreview) {
        GhostObject = Instantiate(PlacePreview, Vector3.zero, Quaternion.identity);
        GhostObject.name = PlacePreview.name;
        DisplayingGhost = true;
    }

    // Remove Display
    public void removePreview() {
        Destroy(GhostObject);
        GhostObject = null;
        DisplayingGhost = false;
    }

    public void RotateTile()
    {
        // Rotate Ghost Object
        if (GhostObject)
        {
            // Get Total Number of Rotations
            
            
        }
    }
    
    // Place Tile
    public void placeTile(GameObject Prefab) {   
        
        // Check if Tile doesn't exist in Position
        foreach (var x in TilePositions) {
            if (GhostObject.transform.position == x.Value.ObjectPosition) {
                return;
            }
        }
        
        // Create Tile Class and Add it
        ObjectClass Tile = new ObjectClass();
        
        // Set Information
        Tile.ObjectPosition = GhostObject.transform.position;
        Tile.ObjectID = ReturnPrefabID();
        
        // Check for rotations
        foreach (Transform Child in GhostObject.transform)
        {
            Tile.RotationObjects.Add(Child.gameObject);
        }
        
        // Create & Add
        Tile.Object = Instantiate(Prefab, GhostObject.transform.position, GhostObject.transform.rotation);
        Tile.Object.name = GhostObject.name;
        TilePositions.Add(Tile.ObjectID, Tile);
      
    }
    
}
