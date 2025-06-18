using System;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;
using Random = System.Random;

public class GridPlacement : MonoBehaviour
{
    
    // Data
    
    Dictionary<string, ObjectClass> Objects = new Dictionary<string, ObjectClass>();
    Dictionary<string, ConveyerClass> Conveyers = new Dictionary<string, ConveyerClass>();
    
    // Variables
    
    public float gridSize;
    public Camera cam;
    public AnimatorController ConveyerLink1Controller;
    public AnimatorController ConveyerLink2Controller;
    public AnimatorController ConveyerLink3Controller;
    public AnimatorController ConveyerLink4Controller;
    public AnimatorController ConveyerLink5Controller;
    public AnimatorController ConveyerLink6Controller;

    private Vector2Int[] directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };
    
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
        PreviewGhostRenderer.flipX = false;
        float zRot = PreviewGhost.Object.transform.eulerAngles.z;
        if (Mathf.Approximately(zRot, 180f))
        {
            PreviewGhostRenderer.flipY = true;
        }
        switch (PreviewGhost.ObjectItem.ItemType)
        {
            case "Conveyer":
                if (Mathf.Approximately(zRot, 180f))
                {
                    PreviewGhost.OutputDirection = Vector2.left;
                }
                if (Mathf.Approximately(zRot, 90f))
                {
                    PreviewGhost.OutputDirection = Vector2.up;
                }
                if (Mathf.Approximately(zRot, 0f))
                {
                    PreviewGhost.OutputDirection = Vector2.right;
                }
                if (Mathf.Approximately(zRot, 270))
                {
                    PreviewGhost.OutputDirection = Vector2.down;
                }
                return;
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
                Vector2Int gridPos = new Vector2Int(
                    Mathf.RoundToInt(ConveyerObject.Object.transform.position.x),
                    Mathf.RoundToInt(ConveyerObject.Object.transform.position.y)
                );
                ConveyerObject.Gridpos = gridPos;
                ConveyerObject.OutputDirection = PreviewGhost.OutputDirection;
                Conveyers.Add(ConveyerObject.ObjectID, ConveyerObject);
                CheckConveyer(ConveyerObject);
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

    public void CheckConveyer(ConveyerClass Conveyer)
    {
        foreach (var dir in directions)
        {
            Vector2Int neighborpos = Conveyer.Gridpos + dir;
            foreach (var item in Conveyers.Values)
            {
                if (item.Gridpos == neighborpos)
                {
                    if (dir == Conveyer.OutputDirection)
                    {
                        if (item.OutputDirection != -Conveyer.OutputDirection && Conveyer.OutputDirection != item.OutputDirection)
                        {
                            RotateConveyerCorrectly(Conveyer, item, dir);
                        }
                    } else if (item.OutputDirection == -dir)
                    {
                        if (Conveyer.OutputDirection != item.OutputDirection)
                        {
                            RotateConveyerCorrectly(Conveyer, Conveyer, dir);
                        }
                    }
                }
            }
        }
    }

    public enum ConveyerDirection { Up, Down, Left, Right }
    
    public ConveyerDirection getDirection(Vector2 Dir)
    {   
        if (Dir == Vector2.left) return ConveyerDirection.Left;
        if (Dir == Vector2.right) return ConveyerDirection.Right;
        if (Dir == Vector2.up) return ConveyerDirection.Up;
        if (Dir == Vector2.down) return ConveyerDirection.Down;
        throw new ArgumentException("Unknown direction: " + Dir);
    }
    
    public void RotateConveyerCorrectly(ConveyerClass Conveyer, ConveyerClass ConveyerToRotate, Vector2Int Direction)
    {
        ConveyerToRotate.Object.transform.rotation = Quaternion.identity;
        var SpriteRenderer = Conveyer.Object.GetComponent<SpriteRenderer>();
        SpriteRenderer.flipY = false;
        SpriteRenderer.flipX = false;
        var animator = ConveyerToRotate.Object.GetComponent<Animator>();
        // Rotation Directions
        print(getDirection(ConveyerToRotate.OutputDirection));
        print(Direction);
        switch (getDirection(ConveyerToRotate.OutputDirection))
        {
            case ConveyerDirection.Up:
                animator.runtimeAnimatorController = ConveyerLink3Controller;
                if (Direction == Vector2Int.right)
                {
                    animator.runtimeAnimatorController = ConveyerLink4Controller;
                }
                return;
            case ConveyerDirection.Down:
                animator.runtimeAnimatorController = ConveyerLink2Controller;
                if (Direction == Vector2Int.left)
                {
                    animator.runtimeAnimatorController = ConveyerLink5Controller;
                    ConveyerToRotate.Object.transform.Rotate(0,0,270f);
                }
                return;
            case ConveyerDirection.Left:
                animator.runtimeAnimatorController = ConveyerLink1Controller;
                if (Direction == Vector2Int.up)
                {
                    animator.runtimeAnimatorController = ConveyerLink6Controller;
                }
                return;
            case ConveyerDirection.Right:
                if (Direction == Vector2Int.down)
                {
                    animator.runtimeAnimatorController = ConveyerLink5Controller;
                }
                else
                {
                    animator.runtimeAnimatorController = ConveyerLink3Controller;
                    ConveyerToRotate.Object.transform.Rotate(0, 0,270);
                }
                return;
            default:
                return;
        }
    }
    
}
