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
    
    // List of Current Tiles
    Dictionary<string, Vector3> TilePositions = new Dictionary<string, Vector3>();

    // Update is called once per frame
    void Update() {
        // Read Cursor Position, Add Mouse Offset, Set Ghost Object & Clamp to Grid
        if (DisplayingGhost != false) {
            var ObjectWorldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            ObjectWorldPos.x = ObjectWorldPos.x + 1;
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
            GeneratedID += UnityEngine.Random.Range(0, Characters.Length);
            x += 1;
        }
        return GeneratedID;
    }
    
    // Display Placing Tile
    public void setPreview(GameObject PlacePreview) {
        GhostObject = Instantiate(PlacePreview, Vector3.zero, Quaternion.identity);
        DisplayingGhost = true;
    }

    // Remove Display
    public void removePreview() {
        Destroy(GhostObject);
        GhostObject = null;
        DisplayingGhost = false;
    }
    
    // Place Tile
    public void placeTile(GameObject Prefab) {   
        // Check if Tile doesn't exist in Position
        foreach (var x in TilePositions) {
            if (GhostObject.transform.position == x.Value) {
                return;
            }
        }
        Instantiate(Prefab, GhostObject.transform.position, GhostObject.transform.rotation);
        TilePositions.Add(ReturnPrefabID(), GhostObject.transform.position);
    }
    
}
