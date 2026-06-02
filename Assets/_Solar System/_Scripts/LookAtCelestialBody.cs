using UnityEngine;
using System.Collections;
using TMPro;

public class LookAtCelestialBody : MonoBehaviour {

    [Tooltip("Reference to our camera controller")]
    private CameraFlightMotor _editorCameraScript; // Reference to the camera controller script

    [Tooltip("This is the object that the script's game object will look at by default")]
    public GameObject defaultTarget; // the default target that the camera should look at

    [Tooltip("This is the object that the script's game object is currently look at based on the player clicking on a gameObject")]
    public GameObject currentTarget; // the target that the camera should look at

    [Tooltip("This is to cache de Main camera for performance improvement")]
    public Camera _mainCamera; // the target that the camera should look at

    [Header("Search UI Settings")]
    [SerializeField] private TMP_InputField _searchInputField; // Drag your UI Input Field here in the Inspector

    void Awake()
    {
        // Check if the main camera is assigned, if not, find it automatically
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;

            if (_mainCamera == null)
            {
                Debug.LogError("LookAtCelestialBody: No main camera found.");
            }
        }

        if (_mainCamera != null)
        {
            _editorCameraScript = _mainCamera.GetComponent<CameraFlightMotor>();

            if (_editorCameraScript == null)
            {
                Debug.LogError("LookAtCelestialBody: No UnityEditorCamera script found on the main camera.");
            }
        }
    }

    void Start()
    {
        if (defaultTarget == null)
        {
            defaultTarget = this.gameObject;
        }

        if (currentTarget == null)
        {
            currentTarget = this.gameObject;
        }
    }

    // Update happens constantly as your game is running
    void Update()
    {
        // PERFORMANCE ADJUSTMENT: We safely evaluate mouse clicks directly. 
        // If the search field is active, Unity's event system automatically blocks raycasts,
        // so we don't need to strain the CPU checking it every single frame!
        TargetCelestialBody();
    }

    void TargetCelestialBody()
    {
        // If the user has actively clicked inside the text input box and is typing,
        // we immediately halt this entire method so mouse clicks don't shoot rays into space!
        if (_searchInputField != null && _searchInputField.isFocused)
        {
            return;
        }


        // if primary mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentTarget = hit.collider.gameObject;
                Debug.Log($"Target changed to: {currentTarget.name}");

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

            if (_editorCameraScript != null && defaultTarget != null)
            {
                _editorCameraScript.currenTarget = defaultTarget.transform;
            }
        }
    }

    // Public method that our UI Search Bar will execute when the user submits text
    public void SearchForPlanet(string textInput)
    {
        if (string.IsNullOrWhiteSpace(textInput))
        {
            return;
        }

        // We find every GameObject in our scene that has a collider component attached
        Collider[] allSceneObjects = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (Collider potentialTarget in allSceneObjects)
        {
            // Compare the names ignoring case constraints cleanly
            if (potentialTarget.gameObject.name.Equals(textInput, System.StringComparison.OrdinalIgnoreCase))
            {
                currentTarget = potentialTarget.gameObject;
                Debug.Log($"Search system successfully found target match: {currentTarget.name}");

                if (_editorCameraScript != null)
                {
                    _editorCameraScript.currenTarget = currentTarget.transform;

                    // FIXED CALL: We execute the optimized standalone method directly 
                    // without passing fake or expensive input context structures!
                    _editorCameraScript.ExecuteFocus();
                }
                break;
            }
        }
    }
}