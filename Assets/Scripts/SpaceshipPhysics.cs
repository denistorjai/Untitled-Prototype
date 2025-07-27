using UnityEngine;

public class SpaceshipPhysics : MonoBehaviour
{
    
    public Rigidbody2D rb;
    public float force;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.AddForce(Vector2.up * force);
        rb.AddTorque(5f);
    }

    // Update is called once per frame
    void Update()
    { 
        rb.AddTorque(0.4f * Time.deltaTime);
    }
    
}
