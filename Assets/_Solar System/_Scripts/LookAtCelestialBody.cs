using UnityEngine;
using System.Collections;

public class LookAtCelestialBody : MonoBehaviour {

    [Tooltip("Reference to our camera controller")]
    private UnityEditorCamera _editorCameraScript; // Reference to the camera controller script

    [Tooltip("This is the object that the script's game object will look at by default")]
    public GameObject defaultTarget; // the default target that the camera should look at

    [Tooltip("This is the object that the script's game object is currently look at based on the player clicking on a gameObject")]
    public GameObject currentTarget; // the target that the camera should look at

    [Tooltip("This is to cache de Main camera for performance improvement")]
    public Camera _mainCamera; // the target that the camera should look at

    void Awake()
    {
        // Check if the main camera is assigned, if not, find it automatically
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;

            // If _mainCamera is still null for any reason, log an error to let us know
            if (_mainCamera == null)
            {
                Debug.LogError("LookAtCelestialBody: No main camera found. Please assign a Camera in the Inspector or tag a Camera as 'MainCamera'.");
            }
        }

        // If we found our main camera, try to grab our custom movement script attached to it
        if (_mainCamera != null)
        {
            _editorCameraScript = _mainCamera.GetComponent<UnityEditorCamera>();

            // Log an error if the movement script is missing from the camera component list
            if (_editorCameraScript == null)
            {
                Debug.LogError("LookAtCelestialBody: No UnityEditorCamera script found on the main camera. Please ensure the main camera has a UnityEditorCamera component.");
            }
        }
    }

    void Start()
    {
        // If no default target is set, default back to the parent GameObject this script is on
        if (defaultTarget == null)
        {
            defaultTarget = this.gameObject;
            Debug.Log("defaultTarget target not specified. Defaulting to parent GameObject");
        }

        // If no current target is set, default back to the parent GameObject as well
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
        TargetCelestialBody();
    }

    void TargetCelestialBody()
    {
        // if primary mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // determine the ray from the camera to the mousePosition
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            // cast a ray to see if it hits any gameObjects
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentTarget = hit.collider.gameObject;
                Debug.Log($"Target changed to: {currentTarget.name}");

                // THE HANDOFF: Tell our editor type camera script that this is the new target
                // We pass the reference directly over so the camera knows what to move towards
                if (_editorCameraScript != null)
                {
                    _editorCameraScript.currenTarget = currentTarget.transform;
                }
            }
        }
        // if the second mouse button is pressed, reset back to the default target
        else if (Input.GetMouseButtonDown(1))
        {
            currentTarget = defaultTarget;
            Debug.Log("defaultTarget changed to " + currentTarget.name);

            // Update our editor camera script to use the default target too
            if (_editorCameraScript != null && defaultTarget != null)
            {
                _editorCameraScript.currenTarget = defaultTarget.transform;
            }
        }

        // The forced Quaternion.Slerp rotation system was removed from this method!
        // This ensures the selection script does not fight the camera's orbiting controls.
    }
}