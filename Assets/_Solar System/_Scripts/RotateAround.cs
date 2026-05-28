using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

    [Tooltip("This is the object that the script's game object will rotate around")]
    public Transform target; // the object to rotate around

    [Tooltip("This is the speed at which the object rotates")]
    public float speed; // Changed to float for precise speed control

    void Start()
    {
        if (target == null)
        {
            target = this.gameObject.transform;
            Debug.Log("RotateAround target not specified. Defaulting to this GameObject");
        }
    }

    // FIXED: Running our rotation inside LateUpdate ensures the planet moves 
    // right before the camera calculates its tracking. This delivers 100% stable,
    // silky-smooth movement without needing any heavy Rigidbody components!
    void LateUpdate()
    {
        RotateObject();
    }

    private void RotateObject()
    {
        // RotateAround takes three arguments:
        // 1. The Vector position to rotate around
        // 2. The axis vector to rotate around
        // 3. The degrees to rotate per frame using standard Time.deltaTime
        transform.RotateAround(target.position, target.up, speed * Time.deltaTime);
    }
}
