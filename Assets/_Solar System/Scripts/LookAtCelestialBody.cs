using UnityEngine;
using System.Collections;

public class LookAtCelestialBody : MonoBehaviour {

    [Tooltip("This is the object that the script's game object will look at by default")]
    public GameObject defaultTarget; // the default target that the camera should look at

    [Tooltip("This is the object that the script's game object is currently look at based on the player clicking on a gameObject")]
    public GameObject currentTarget; // the target that the camera should look at

    [Tooltip("This is to cache de Main camera for performance improvement")]
    public Camera _mainCamera; // the target that the camera should look at

    [Tooltip("This is the speed of the camera rotation")]
    [SerializeField] float rotationSpeed = 5f; // Speed at which the camera rotates to look at the target

    void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;

            // If _mainCamera is still null for any reason, log an error
            if (_mainCamera == null)
            {
                Debug.LogError("LookAtCelestialBody: No main camera found. Please assign a Camera in the Inspector or tag a Camera as 'MainCamera'.");
            }
        }
    }

    void Start () {
		if (defaultTarget == null) 
		{
            defaultTarget = this.gameObject;
			Debug.Log ("defaultTarget target not specified. Defaulting to parent GameObject");
		}

        if (currentTarget == null)
        {
            currentTarget = this.gameObject;
            Debug.Log("currentTarget target not specified. Defaulting to parent GameObject");
        }
    }
	
	// Update is called once per frame
    // For clarity, Update happens constantly as your game is running
    void Update()
    {
        TargetClestialBody();
    }

    void TargetClestialBody()
    {
        // if primary mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // determine the ray from the camera to the mousePosition
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            // cast a ray to see if it hits any gameObjects
            //RaycastHit[] hits;
            //hits = Physics.RaycastAll(ray);

            // if there are hits
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentTarget = hit.collider.gameObject;
                Debug.Log($"Target changed to: {currentTarget.name}");
            }
        }
        else if (Input.GetMouseButtonDown(1)) // if the second mouse button is pressed
        {
            currentTarget = defaultTarget;
            Debug.Log("defaultTarget changed to " + currentTarget.name);
        }

        // if a currentTarget is set, then look at it
        if (currentTarget != null)
        {
            // transform here refers to the attached gameobject this script is on.
            // the LookAt function makes a transform point it's Z axis towards another point in space
            // In this case it is pointing towards the target.transform
            //transform.LookAt(currentTarget.transform);
            
            // Calculate the direction to the target
            Vector3 directionToTarget = currentTarget.transform.position - transform.position;

            // Calculate the rotation needed to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else // reset the look at back to the default
        {
            currentTarget = defaultTarget;
            Debug.Log("defaultTarget changed to " + currentTarget.name);
        }
    }
}
