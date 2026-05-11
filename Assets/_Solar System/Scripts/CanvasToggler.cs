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
        DisplayInformation();
    }

    private void CheckForCanvas()
    {
        if (targetCanvas == null)
        {
            // Only search if we haven't manually assigned it in the Inspector
            targetCanvas = GetComponent<Canvas>();

            if (targetCanvas == null)
            {
                Debug.LogError("CanvasToggler: No Canvas found on this GameObject. Please assign a Canvas in the Inspector or add a Canvas component.");
            }
        }
    }

    private void DisplayInformation()
    {
        if (targetCanvas != null)
        {
            // Toggle the canvas on or off when the F key is pressed
            targetCanvas.enabled = Input.GetKeyDown(KeyCode.F) ? !targetCanvas.enabled : targetCanvas.enabled;
        }
    }
}
