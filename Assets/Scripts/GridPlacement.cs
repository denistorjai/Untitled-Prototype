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
    Dictionary<string, ConveyerItem> ConveyerItems = new Dictionary<string, ConveyerItem>();
    
    // Variables
    
    public float gridSize;
    public Camera cam;
    public AnimatorController[] ConveyerAnimationControllers;
    public MinerManager MinerManager;
    
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
                ConveyerObject.ObjectSize = ConveyerObject.Object.GetComponent<SpriteRenderer>().bounds.size;
                print(ConveyerObject.ObjectSize);
                PreviewGhost = ConveyerObject;
                PreviewGhostRenderer = ConveyerObject.Object.GetComponent<SpriteRenderer>();
                SyncAnimation(ConveyerObject);
                return;
            case "Miner":
                MinerClass MinerObject = new MinerClass();
                MinerObject.Object = Instantiate(item.ObjectPrefab);
                MinerObject.ObjectItem = item;
                MinerObject.ObjectID = ReturnID();
                MinerObject.Object.name = "PreviewObject";
                MinerObject.ObjectSize = MinerObject.Object.GetComponent<SpriteRenderer>().bounds.size;
                PreviewGhost = MinerObject;
                PreviewGhostRenderer = MinerObject.Object.GetComponent<SpriteRenderer>();
                SyncAnimation(MinerObject);
                return;
            default:
                ObjectClass Object = new ObjectClass();
                Object.Object = Instantiate(item.ObjectPrefab);
                Object.ObjectItem = item;
                Object.ObjectID = ReturnID();
                Object.ObjectSize = Object.Object.GetComponent<SpriteRenderer>().bounds.size;
                PreviewGhost = Object;
                PreviewGhostRenderer = Object.Object.GetComponent<SpriteRenderer>();
                SyncAnimation(Object);
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

    private void Start()
    {
        // Get Minerals
        Dictionary<string, MineralClass> Minerals = MinerManager.GetMinerals();
        foreach (var Item in Minerals.Values)
        {
            ObjectMineralClass MineralObject = new ObjectMineralClass();
            MineralObject.Mineral = Item;
            MineralObject.ObjectID = ReturnID();
            MineralObject.Object = MineralObject.Mineral.MineralObject;
            Vector2Int Gridpos = new Vector2Int(
                Mathf.RoundToInt(MineralObject.Object.transform.position.x),
                Mathf.RoundToInt(MineralObject.Object.transform.position.y)
            );
            MineralObject.Gridpos = Gridpos;
            MineralObject.Object.name = "MineralObject";
            MineralObject.ActiveObject = true;
            MineralObject.ObjectSize = MineralObject.Object.GetComponent<SpriteRenderer>().bounds.size;
            Objects.Add(MineralObject.ObjectID, MineralObject);
            print("Object");
            print(MineralObject.ObjectID);
            print(MineralObject.Gridpos);
        }
    }

    // TO DO TOMORROW: ADD A LIST OF CONVEYERABLE CLASSES, CONVERT TO CONVEYERITERM CLASS, MAKE SPAWNER, USE VECTOR2 TO DIRECTIOn, VECTORINT FOR GRID POS, MOVE ITEM TO NEXT GRIDPOS, AND KEEP DOING THAT BASED ON CONVEYERPOSITION
    
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
            if (Object.Value.Object.name != "MineralObject")
            {
                var PreviewGhostGridPos = new Vector2Int(Mathf.RoundToInt(PreviewGhost.Object.transform.position.x), Mathf.RoundToInt(PreviewGhost.Object.transform.position.y));
                if (Object.Value.Gridpos == PreviewGhostGridPos)
                {
                    return;
                }
            }
            
            if (PreviewGhost.ObjectItem.ItemName == "Miner")
            {
                print("OBJECT VVVVV");
                var PreviewGhostGridPos = new Vector2Int(Mathf.RoundToInt(PreviewGhost.Object.transform.position.x), Mathf.RoundToInt(PreviewGhost.Object.transform.position.y));
                print(Object.Value.ObjectID);
                print(Object.Value.Gridpos);
                print(PreviewGhostGridPos);
                if (Object.Value.Gridpos != PreviewGhostGridPos)
                {
                    return;
                }
            }
            
            // TO DO: Fix Intersection Points Later
            
            //if (CheckIntersect(Object.Value.Gridpos, Object.Value.ObjectSize, PreviewGhostGridPos, PreviewGhost.ObjectSize) == false)
            //  {
            //    print(CheckIntersect(Object.Value.Gridpos, Object.Value.ObjectSize, PreviewGhostGridPos, PreviewGhost.ObjectSize));
            //    return;
            //  }
        }
        switch (PreviewGhost.ObjectItem.ItemName)
        {
            case "Conveyer":
                ConveyerClass ConveyerObject = new ConveyerClass();
                ConveyerObject.Object = Instantiate(PreviewGhost.Object, PreviewGhost.Object.transform.position, PreviewGhost.Object.transform.rotation);
                ConveyerObject.ObjectID = ReturnID();
                ConveyerObject.Object.name = PreviewGhost.ObjectItem.ItemName;
                Objects.Add(ConveyerObject.ObjectID, ConveyerObject);
                Vector2Int ConveyerGridPos = new Vector2Int(
                    Mathf.RoundToInt(ConveyerObject.Object.transform.position.x),
                    Mathf.RoundToInt(ConveyerObject.Object.transform.position.y)
                );
                ConveyerObject.Gridpos = ConveyerGridPos;
                ConveyerObject.OutputDirection = PreviewGhost.OutputDirection;
                Conveyers.Add(ConveyerObject.ObjectID, ConveyerObject);
                CheckConveyer(ConveyerObject);
                ConveyerObject.ActiveObject = true;
                SyncAnimation(ConveyerObject);
                return;
            case "Miner":
                MinerClass MinerObject = new MinerClass();
                MinerObject.Object = Instantiate(PreviewGhost.Object, PreviewGhost.Object.transform.position, PreviewGhost.Object.transform.rotation);
                MinerObject.ObjectID = ReturnID();
                MinerObject.Object.name = PreviewGhost.ObjectItem.ItemName;
                Objects.Add(MinerObject.ObjectID, MinerObject);
                Vector2Int MinerGridPos = new Vector2Int(
                    Mathf.RoundToInt(MinerObject.Object.transform.position.x),
                    Mathf.RoundToInt(MinerObject.Object.transform.position.y)
                );
                MinerObject.Gridpos = MinerGridPos;
                return;
            default:
                ObjectClass Object = new ObjectClass();
                Object.Object = Instantiate(PreviewGhost.Object, PreviewGhost.Object.transform.position, PreviewGhost.Object.transform.rotation);
                Object.ObjectID = ReturnID();
                Object.Object.name = PreviewGhost.ObjectItem.ItemName;
                Objects.Add(Object.ObjectID, Object);
                Vector2Int GridPos = new Vector2Int(
                    Mathf.RoundToInt(PreviewGhost.Object.transform.position.x),
                    Mathf.RoundToInt(PreviewGhost.Object.transform.position.y)
                );
                Object.Gridpos = GridPos;
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
        switch (getDirection(ConveyerToRotate.OutputDirection))
        {
            case ConveyerDirection.Up:
                animator.runtimeAnimatorController = ConveyerAnimationControllers[2];
                if (Direction == Vector2Int.right)
                {
                    animator.runtimeAnimatorController = ConveyerAnimationControllers[3];
                }
                return;
            case ConveyerDirection.Down:
                animator.runtimeAnimatorController = ConveyerAnimationControllers[1];
                if (Direction == Vector2Int.left)
                {
                    animator.runtimeAnimatorController = ConveyerAnimationControllers[4];
                    ConveyerToRotate.Object.transform.Rotate(0,0,270f);
                }
                return;
            case ConveyerDirection.Left:
                animator.runtimeAnimatorController = ConveyerAnimationControllers[0];
                if (Direction == Vector2Int.up)
                {
                    animator.runtimeAnimatorController = ConveyerAnimationControllers[5];
                }
                return;
            case ConveyerDirection.Right:
                if (Direction == Vector2Int.down)
                {
                    animator.runtimeAnimatorController = ConveyerAnimationControllers[4];
                }
                else
                {
                    animator.runtimeAnimatorController = ConveyerAnimationControllers[2];
                    ConveyerToRotate.Object.transform.Rotate(0, 0,270);
                }
                return;
            default:
                return;
        }
    }

    // Finish Check intersct Function later
    bool CheckIntersect(Vector2Int GridposA, Vector3 SizeA, Vector2Int GridposB, Vector3 SizeB)
    {
        return true;
    }
    
}
