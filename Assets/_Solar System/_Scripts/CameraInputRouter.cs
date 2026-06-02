using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputRouter : MonoBehaviour
{
    // These auto-properties store raw data from the mouse and keyboard inputs data-safely
    // Other components can read these freely but cannot overwrite them from the outside
    public Vector2 MouseDelta { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public float VerticalInput { get; private set; }
    public float ScrollInput { get; private set; }
    public bool IsOrbiting { get; private set; }
    public bool IsPanning { get; private set; }

    // Architecture Alerts: These isolated event tracks let other systems know exactly when a key phase executes
    public System.Action OnFocusPerformed;
    public System.Action OnUnfocusPerformed;
    public System.Action OnOpenEncyclopediaPerformed;

    // Catcher Methods
    // We need public methods that the playerInput component can call. These use InputAction.CallbackContext to "read" the values

    public void OnLook(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the mouse movement as a 2D coordinate 
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnOrbitClick(InputAction.CallbackContext context)
    {
        // ReadValueAsButton tells the script "is the button currently pressed or not?"
        IsOrbiting = context.ReadValueAsButton();
    }

    public void OnPan(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the mouse movement as a 2D coordinate 
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnPanClick(InputAction.CallbackContext context)
    {
        // ReadValueAsButton tells the script "is the button currently pressed or not?"
        IsPanning = context.ReadValueAsButton();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the scroll wheel movement as a Vector2 coordinate
        // We extract the .y axis because scroll wheels only move vertically up and down
        ScrollInput = context.ReadValue<float>();
    }

    public void OnFlyVertical(InputAction.CallbackContext context)
    {
        // We read the vertical input for ascending and descending
        VerticalInput = context.ReadValue<float>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the WASD keyboard composite layout as a 2D Vector2 coordinate
        // X will store Left/Right (-1 to 1) and Y will store Forward/Backward (-1 to 1)
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        // Only execute when the button is first tapped down ('performed' phase)
        if (context.performed)
        {
            OnFocusPerformed?.Invoke();
        }
    }

    public void OnUnfocus(InputAction.CallbackContext context)
    {
        // Only execute when the button is first tapped down ('performed' phase)
        if (context.performed)
        {
            OnUnfocusPerformed?.Invoke();
        }
    }

    public void OnOpenEncyclopedia(InputAction.CallbackContext context)
    {
        // Only execute when the button is first tapped down ('performed' phase)
        if (context.performed)
        {
            OnOpenEncyclopediaPerformed?.Invoke();
        }
    }
}
