using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasToggler : MonoBehaviour
{
    [Tooltip("This is the canvas used to display the planet's information")]
    [SerializeField] private Canvas targetCanvas;

    void Start()
    {
        CheckForCanvas();

    }

    void Update()
    {
        // We only want to run the toggle logic IF the key is actually pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCanvas();
        }
    }

    private void CheckForCanvas()
    {
        if (targetCanvas == null)
        {
            // Try to find a Canvas component on the same GameObject
            //targetCanvas = GetComponent<Canvas>();

            // If still null, try finding it in children
            if (targetCanvas == null)
            {
                targetCanvas = GetComponentInChildren<Canvas>();
            }

            if (targetCanvas == null)
            {
                Debug.LogError($"{gameObject.name} (CanvasToggler): No Canvas found! Drag one into the Inspector.");
            }
        }
    }

    private void ToggleCanvas()
    {
        if (targetCanvas != null)
        {
            // Toggle the canvas on or off when the F key is pressed
            // Simplified toggle: sets enabled to the opposite of what it currently is
            targetCanvas.enabled = !targetCanvas.enabled;
            Debug.Log($"Canvas is now: {(targetCanvas.enabled ? "ON" : "OFF")}");

  
        }
    }
}
