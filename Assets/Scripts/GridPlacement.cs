using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GridPlacement : MonoBehaviour
{
    
    public static GridPlacement Instance;
    
    // Data
    
    Dictionary<string, ObjectClass> Objects = new Dictionary<string, ObjectClass>();
    Dictionary<string, ConveyerClass> Conveyers = new Dictionary<string, ConveyerClass>();
    Dictionary<string, ConveyerItem> ConveyerItems = new Dictionary<string, ConveyerItem>();
    Dictionary<string, MinerClass> Miners = new Dictionary<string, MinerClass>();
    Dictionary<string, ObjectMineralClass> ObjectMinerals = new Dictionary<string, ObjectMineralClass>();
    
    // Variables
    
    public float gridSize;
    public Camera cam;
    public AnimatorController[] ConveyerAnimationControllers;
    public MinerManager MinerManager;
    public PlayerManager PlayerManager;
    public GameObject SpaceshipPrefab;
    public AudioSource audioSource;
    public AudioClip placesound;
    public GameObject OutputIndicator;

    public GameObject ActiveIndicator;
    
    private Vector2Int[] directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };
    
    // Ghost Preview Methods

    private ObjectClass PreviewGhost;
    private SpriteRenderer PreviewGhostRenderer;
    
    public void setPreview(ItemClass item)
    {
        switch (item.ItemType)
        {
            case "Conveyer":
                ConveyerClass ConveyerObject = new ConveyerClass();
                ConveyerObject.Object = Instantiate(item.ObjectPrefab);
                ConveyerObject.ObjectItem = item;
                ConveyerObject.ObjectID = ReturnID();
                ConveyerObject.Object.name = "PreviewObject";
                ConveyerObject.ObjectSize = ConveyerObject.Object.GetComponent<SpriteRenderer>().bounds.size;
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
        Destroy(PreviewGhost.Object);
        PreviewGhost = null;
    }

    public void rotatePreview()
    {
        Vector2 DesiredRotation = Vector2.left;
        if (PreviewGhost.OutputDirection == Vector2.left)
        {
            DesiredRotation = Vector2.down;
        }
        if (PreviewGhost.OutputDirection == Vector2.down)
        {
            DesiredRotation = Vector2.right;
        }
        if (PreviewGhost.OutputDirection == Vector2.right)
        {
            DesiredRotation = Vector2.up;
        }

        if (PreviewGhost.OutputDirection == Vector2.up)
        {
            DesiredRotation = Vector2.left;
        }
        RotateToDesired(DesiredRotation, PreviewGhost);
    }

    public void RotateToDesired(Vector2 DesiredRotation, ObjectClass ObjectToRotate)
    {
        // Output Direction
        ObjectToRotate.OutputDirection = DesiredRotation;
        // Visuals
        ObjectToRotate.Object.GetComponent<SpriteRenderer>().flipY = false;
        ObjectToRotate.Object.GetComponent<SpriteRenderer>().flipX = false;
        switch (ObjectToRotate.ObjectItem.ItemType)
        {
            case "Conveyer":
                if (ObjectToRotate.OutputDirection == Vector2.left)
                {
                    ObjectToRotate.Object.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                    ObjectToRotate.Object.GetComponent<SpriteRenderer>().flipY = true;
                }
                if (ObjectToRotate.OutputDirection == Vector2.down)
                {
                    ObjectToRotate.Object.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    ObjectToRotate.Object.GetComponent<SpriteRenderer>().flipY = true;
                    ObjectToRotate.Object.GetComponent<SpriteRenderer>().flipX = true;
                }
                if (ObjectToRotate.OutputDirection == Vector2.right)
                {
                    ObjectToRotate.Object.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                if (ObjectToRotate.OutputDirection == Vector2.up)
                {
                    ObjectToRotate.Object.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                }
                return;
        }
    }
    
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Add Space Ship
        SpaceShip Spaceship = new SpaceShip();
        Spaceship.Object = SpaceshipPrefab;
        Spaceship.ObjectID = ReturnID();
        Spaceship.Gridpos = new Vector2Int(0, 0);
        Spaceship.ActiveObject = true;
        Spaceship.AllowConveyerItems = true;
        Spaceship.AllowedConveyerItems = 100000000;
        Spaceship.ObjectType = "SpaceShip";
        Objects.Add(Spaceship.ObjectID, Spaceship);

    }
    
    private float Cooldown;

    public void StartRound()
    {
        // Get Minerals
        Dictionary<string, MineralClass> Minerals = MinerManager.Instance.GetMinerals();
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
            ObjectMinerals.Add(MineralObject.ObjectID, MineralObject);
        }
    }
    
    void Update() 
    {
        if (PreviewGhost != null)
        {
            var MousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            MousePos.z = 0;
            if (MousePos.x > -18 && MousePos.x < 18 && MousePos.y > -18 && MousePos.y < 18)
            {
                PreviewGhost.Object.transform.position = GridClamp(MousePos);
            }
            
            // Output Indicator
            if (ActiveIndicator == null)
            {
                ActiveIndicator = Instantiate(OutputIndicator);
                ActiveIndicator.transform.position = PreviewGhost.Object.transform.position;
            }
            else
            {
                // Calculate Offset and Direction
                Vector3 Offset = Vector3.zero;
                ActiveIndicator.GetComponent<SpriteRenderer>().flipY = false;
                if (PreviewGhost.OutputDirection == Vector2.left)
                {
                    Offset = new Vector3(-0.9f,0,0);
                    ActiveIndicator.transform.rotation = Quaternion.Euler(0,0,90);
                }
                if (PreviewGhost.OutputDirection == Vector2.right)
                {
                    Offset = new Vector3(0.9f,0,0);
                    ActiveIndicator.transform.rotation = Quaternion.Euler(0,0,270);
                }
                if (PreviewGhost.OutputDirection == Vector2.up)
                {
                    Offset = new Vector3(0,0.9f,0);
                    ActiveIndicator.transform.rotation = Quaternion.Euler(0,0,0);
                }
                if (PreviewGhost.OutputDirection == Vector2.down)
                {
                    Offset = new Vector3(0,-0.9f,0);
                    ActiveIndicator.transform.rotation = Quaternion.Euler(0,0,180);
                }
                ActiveIndicator.transform.position = PreviewGhost.Object.transform.position + Offset;
            }
            
        }
        
        // Object & Spawning Management
        Cooldown -= Time.deltaTime;
        
        // Miner Spawning
        if (Cooldown < 0)
        {
            foreach (MinerClass Miner in Miners.Values)
            {
                var OutputPos = Miner.Gridpos + Miner.OutputDirection;
                foreach (ConveyerClass Conveyer in Conveyers.Values)
                {
                    if (Conveyer.Gridpos == OutputPos)
                    {
                        SpawnFuel(Miner, Conveyer);
                    }
                }
            } 
            Cooldown = 3f;
        }
        
        // Conveyer Items
        foreach (ConveyerClass Conveyer in Conveyers.Values)
        {
            foreach (var Item in Conveyer.ConveyerItems.Values)
            {
                var Output = Conveyer.Gridpos + Conveyer.OutputDirection;
                foreach (var ComparingObject in Objects.Values)
                {
                    if (ComparingObject.Gridpos == Output)
                    {
                        if (ComparingObject.ConveyerItems.Count < ComparingObject.AllowedConveyerItems)
                        {
                            if (Item.CurrentTween == null || !Item.CurrentTween.IsActive() || !Item.CurrentTween.IsPlaying())
                            {
                                Item.CurrentTween = Item.ObjectPrefab.transform
                                    .DOMove(GridClamp(ComparingObject.Object.transform.position), 0.4f).SetEase(Ease.Linear).OnComplete(() =>
                                    {
                                        ObjectFunction(Conveyer, ComparingObject, Item);
                                    });
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void ObjectFunction(ObjectClass Object, ObjectClass ComparingObject, ConveyerItem Item)
    {
        ComparingObject.ConveyerItems.Add(Item.ObjectID, Item);
        Object.ConveyerItems.Remove(Item.ObjectID);
        
        switch (ComparingObject.ObjectType)
        {
            case "SpaceShip":
                Destroy(Item.ObjectPrefab);
                PlayerManager.Instance.AddFuel(Item.Fuel.FuelAmount);
                ComparingObject.ConveyerItems.Remove(Item.ObjectID);
                DeleteClass(Item);
                return;
            default:
                return;
        }
        
    }
        
    public void DeleteClass(ConveyerItem item)
    {
        item = null;
    }
    
    // Placing Methods
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlaceObject()
    {
        
        var BlockPlace = false;
        var AllowMiner = false;
        
        foreach (var Object in Objects)
        {
            // Preview Object Position
           Vector2Int PreviewGhostGridpos = new Vector2Int(
                Mathf.RoundToInt(PreviewGhost.Object.transform.position.x),
                Mathf.RoundToInt(PreviewGhost.Object.transform.position.y)
                );

                if (Object.Value.Gridpos == PreviewGhostGridpos) {
                    if (Object.Value.Object.name == "MineralObject") {
                        var Broken = false;
                        foreach (var Item in Objects) {
                            if (Item.Value.Gridpos == Object.Value.Gridpos) {
                                if (Item.Value.Object.name == "Miner") {
                                    BlockPlace = true;
                                    AllowMiner = false;
                                    Broken = true;
                                    break;
                                }
                                else {
                                    AllowMiner = true;
                                }
                            }
                        }
                        if (Broken == true) {
                            break;
                        }
                    } else {
                        BlockPlace = true;
                        break;
                    }
                }
        }
            
        if (BlockPlace) {
            return;
        }

        if (!AllowMiner && PreviewGhost.ObjectItem.ItemType == "Miner") {
            return;
        }
        
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(placesound);
        
        switch (PreviewGhost.ObjectItem.ItemName)
        {
            case "Conveyer":
                ConveyerClass ConveyerObject = new ConveyerClass();
                ConveyerObject.Object = Instantiate(PreviewGhost.Object, PreviewGhost.Object.transform.position, PreviewGhost.Object.transform.rotation);
                ConveyerObject.ObjectItem = PreviewGhost.ObjectItem;
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
                ConveyerObject.ActiveObject = true;
                ConveyerObject.AllowConveyerItems = true;
                ConveyerPlaced();
                SyncAnimation(ConveyerObject);
                return;
            case "Miner":
                MinerClass MinerObject = new MinerClass();
                MinerObject.Object = Instantiate(PreviewGhost.Object, PreviewGhost.Object.transform.position, PreviewGhost.Object.transform.rotation);
                MinerObject.ObjectID = ReturnID();
                MinerObject.Object.name = PreviewGhost.ObjectItem.ItemName;
                MinerObject.OutputDirection = PreviewGhost.OutputDirection;
                Objects.Add(MinerObject.ObjectID, MinerObject);
                Miners.Add(MinerObject.ObjectID, MinerObject);
                Vector2Int MinerGridPos = new Vector2Int(
                    Mathf.RoundToInt(MinerObject.Object.transform.position.x),
                    Mathf.RoundToInt(MinerObject.Object.transform.position.y)
                );
                MinerObject.Gridpos = MinerGridPos;
                SyncAnimation(MinerObject);
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

    public void ConveyerPlaced()
    {
        foreach (ConveyerClass Conveyer in Conveyers.Values)
        {
            var HasDirectConnection = false;
            Vector2 InputDirection = Vector2.zero;
            foreach (var Dir in directions)
            {
                foreach (ConveyerClass ComparingConveyer in Conveyers.Values)
                {
                    if (Conveyer.Gridpos == ComparingConveyer.Gridpos + Dir)
                    {
                        if (ComparingConveyer.Gridpos + ComparingConveyer.OutputDirection == Conveyer.Gridpos)
                        {
                            if (Conveyer.OutputDirection == ComparingConveyer.OutputDirection)
                            {
                                HasDirectConnection = true;
                            }
                            else
                            {
                                InputDirection = ComparingConveyer.OutputDirection;
                            }
                        }
                    }
                }
            }
            if (HasDirectConnection == false && InputDirection != Vector2.zero)
            {
                SetRotationOnConveyer(Conveyer, InputDirection, Conveyer.OutputDirection);
            }
        }
    }

    public void SetRotationOnConveyer(ConveyerClass Conveyer, Vector2 InputDirection, Vector2 OutputDirection)
    {
        Animator conveyerAnimator = Conveyer.Object.GetComponent<Animator>();
        SpriteRenderer conveyerSpriteRenderer = Conveyer.Object.GetComponent<SpriteRenderer>();
        if (InputDirection == Vector2.left)
        {
            if (OutputDirection == Vector2.up)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[3];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 0);
                SyncAnimation(Conveyer);
            }
            if (OutputDirection == Vector2.down)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[1];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 0);
                SyncAnimation(Conveyer);
            }
        }
        if (InputDirection == Vector2.right)
        {
            if (OutputDirection == Vector2.up)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[2];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 0);
                SyncAnimation(Conveyer);
            }
            if (OutputDirection == Vector2.down)
            {
                conveyerSpriteRenderer.flipX = true;
                conveyerSpriteRenderer.flipY = true;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[4];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 90);
                SyncAnimation(Conveyer);
            }
        }
        if (InputDirection == Vector2.down)
        {
            if (OutputDirection == Vector2.left)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[5];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 0);
                SyncAnimation(Conveyer);
            }
            if (OutputDirection == Vector2.right)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[2];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 270);
                SyncAnimation(Conveyer);
            }
        }
        if (InputDirection == Vector2.up)
        {
            if (OutputDirection == Vector2.left)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[0];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 0);
                SyncAnimation(Conveyer);
            }
            if (OutputDirection == Vector2.right)
            {
                conveyerSpriteRenderer.flipX = false;
                conveyerSpriteRenderer.flipY = false;
                conveyerAnimator.runtimeAnimatorController = ConveyerAnimationControllers[4];
                Conveyer.Object.transform.rotation = Quaternion.Euler(0, 0, 0);
                SyncAnimation(Conveyer);
            }
        }
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
        
    }

    // Finish Check intersct Function later
    bool CheckIntersect(Vector2Int GridposA, Vector3 SizeA, Vector2Int GridposB, Vector3 SizeB)
    {
        return true;
    }

    public void SpawnFuel(MinerClass Miner, ConveyerClass Conveyer)
    {
        foreach (ObjectMineralClass Mineral in ObjectMinerals.Values)
        {
            if (Mineral.Gridpos == Miner.Gridpos)
            {
                if (Conveyer.ConveyerItems.Count < 1)
                {
                    var Fuel = Mineral.Mineral.Fuel.ObjectPrefab;
                    var ConveyerItem = new ConveyerItem();
                    ConveyerItem.Fuel = Mineral.Mineral.Fuel;
                    ConveyerItem.ObjectPrefab = Instantiate(Fuel, GridClamp(Conveyer.Object.transform.position), Quaternion.identity);
                    ConveyerItem.ObjectID = ReturnID();
                    Conveyer.ConveyerItems.Add(ConveyerItem.ObjectID, ConveyerItem);
                }
            }
        }
    }
}
