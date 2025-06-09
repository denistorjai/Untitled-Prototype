using System;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;
using Random = System.Random;

public class GridPlacement : MonoBehaviour
{
    
    // Data
    
    Dictionary<string, ObjectClass> Objects = new Dictionary<string, ObjectClass>();
    
    // Variables
    
    public float gridSize;
    public Camera cam;
    
    // Ghost Preview Methods

    private ObjectClass PreviewGhost;
    private SpriteRenderer PreviewGhostRenderer;
    
    public void setPreview(ItemClass item) {
        switch (item.ItemType)
        {
            case "Conveyer":
                ConveyerClass ConveyerObject = new ConveyerClass();
                ConveyerObject.Object = Instantiate(item.ObjectPrefab);
                ConveyerObject.ObjectItem = item;
                ConveyerObject.ObjectID = ReturnID();
                ConveyerObject.Object.name = "PreviewObject";
                PreviewGhost = ConveyerObject;
                PreviewGhostRenderer = ConveyerObject.Object.GetComponent<SpriteRenderer>();
                SyncAnimation(ConveyerObject);
                return;
        }
    }
    
    public void removePreview() {
        PreviewGhost.DeleteObject();
        PreviewGhost = null;
    }

    public void rotatePreview()
    {
        PreviewGhost.Object.transform.Rotate(0,0,90f);
        PreviewGhostRenderer.flipY = false;
        float zRot = PreviewGhost.Object.transform.eulerAngles.z;
        if (Mathf.Approximately(zRot, 180f))
        {
            PreviewGhostRenderer.flipY = true;
        }
    }
    
    void Update() 
    {
        if (PreviewGhost != null)
        {
            var MousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            MousePos.z = 0;
            PreviewGhost.Object.transform.position = GridClamp(MousePos);
        }
    }
    
    // Placing Methods
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlaceObject()
    {
        foreach (var Object in Objects)
        {
            if (Object.Value.Object.transform.position == PreviewGhost.Object.transform.position)
            {
                return;
            }
        }
        switch (PreviewGhost.ObjectItem.ItemName)
        {
            case "Conveyer":
                ConveyerClass ConveyerObject = new ConveyerClass();
                ConveyerObject.Object = Instantiate(PreviewGhost.Object, PreviewGhost.Object.transform.position, PreviewGhost.Object.transform.rotation);
                ConveyerObject.ObjectID = ReturnID();
                ConveyerObject.Object.name = PreviewGhost.ObjectItem.ItemName;
                Objects.Add(ConveyerObject.ObjectID, ConveyerObject);
                ConveyerObject.ActiveObject = true;
                SyncAnimation(ConveyerObject);
                return;
        }
    }
    
    // Use Methods
    
    string ReturnID()
    {
        string Characters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var ID = "";
        for (int i = 0; i < 9; i++) {
            ID += Characters[UnityEngine.Random.Range(0, Characters.Length)];
        }
        return ID;
    }

    Vector3 GridClamp(Vector3 WorldPos)
    {
        return new Vector3( Mathf.Floor(WorldPos.x / gridSize) * gridSize, Mathf.Floor(WorldPos.y / gridSize) * gridSize, WorldPos.z );
    }

    public void SyncAnimation(ObjectClass Object)
    {
        Animator animator = Object.Object.GetComponent<Animator>();
        float globalTime = Time.time;
        float AnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        animator.Play(0,0, (globalTime % AnimationLength)/AnimationLength);
        animator.Update(0f);
    }
    
}
