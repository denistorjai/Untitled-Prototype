using Unity.VisualScripting;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    
    public static CameraHandler Instance;
    public static GameObject PlayerCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayerCamera = this.gameObject;
        }
        else
        {
            Destroy(this);
            Destroy(this.gameObject);
        }
    }
}
