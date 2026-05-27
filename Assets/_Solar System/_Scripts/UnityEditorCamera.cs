using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityEditorCamera : MonoBehaviour
{
    // These store raw data from the mouse
    private Vector2 _mouseDelta;
    private float _scrollInput;
    private float _pitch;
    private Vector3 _targetPosition;

    // These store the YES/NO states of the buttons
    private bool _isOrbiting;
    private bool _isPanning;

    // Sensitivity settings for the camera movement
    [Tooltip("This are the mouse sensitivity settings")]
    [SerializeField] private float _rotationSensitivity_X = 0.1f;
    [SerializeField] private float _rotationSensitivity_Y = 0.1f;
    [SerializeField] private float _panningSensitivity = 0.1f;
    [SerializeField] private float _zoomSensitivity = 0.05f;

    [Tooltip("This are the camera smoothing controls")]
    [SerializeField] private float lerpSpeed = 5f;

    [Tooltip("This are the focus settings")]
    [SerializeField] private float _focusDistance = 15f;
    public Transform currenTarget;

    void Start()
    {
        // Set our phantom target position to start wherever the camera is currently placed
        _targetPosition = transform.position;
    }

    // LateUpdate happens right after all other standard updates have run
    // This is perfect for camera tracking because it prevents unexpected stuttering
    void LateUpdate()
    {
        HandleOrbiting();
        HandlePanning();
        HandleZooming();
        ApplySmoothMovement();
    }

    // Catcher Methods
    // We need public methods that the playerInput component can call. These use InputAction.CallbackContext to "read" the values

    public void OnLook(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the mouse movement as a 2D coordinate 
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnOrbitClick(InputAction.CallbackContext context)
    {
        // ReadValueAsButton tells the script "is the button currently pressed or not?"
        _isOrbiting = context.ReadValueAsButton();
    }

    public void OnPan(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the mouse movement as a 2D coordinate 
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnPanClick(InputAction.CallbackContext context)
    {
        // ReadValueAsButton tells the script "is the button currently pressed or not?"
        _isPanning = context.ReadValueAsButton();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the scroll wheel movement as a Vector2 coordinate
        // We extract the .y axis because scroll wheels only move vertically up and down
        _scrollInput = context.ReadValue<Vector2>().y;
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        // Only execute when the button is first tapped down ('performed' phase)
        if (context.performed && currenTarget != null)
        {
            // We find the direction from the planet to the camera
            Vector3 directionToCamera = (transform.position - currenTarget.position).normalized;

            // Safety Check: If the camera is exactly on top of the planet, back up along the z axis
            if (directionToCamera == Vector3.zero)
            {
                directionToCamera = Vector3.back;
            }

            // Find the planet's radius using its Renderer component
            float planetRadius = 0f;

            // We look for the MeshRenderer first, if it exists, we use its bounds to get the radius
            Renderer planetRenderer = currenTarget.GetComponentInChildren<Renderer>();
            if (planetRenderer != null)
            {
                // Extents.y gives us half the height of the planet, which is a good approximation for the radius. 
                // This works even if the planet is scaled up or down, because the bounds are calculated based on the actual size of the object in the scene.
                planetRadius = planetRenderer.bounds.extents.y;
            }

            // We set our phantom target position to sit outside the planet
            // Total Distance = Planet Radius + Focus Distance ( padding ) so we don't end up inside the planet when we focus
            _targetPosition = currenTarget.position + (directionToCamera * (planetRadius + _focusDistance));

            // We want the camera to look directly at the planet's center 
            // We calculate this from our intended target position so our view angles stay mathematically synchronized
            transform.rotation = Quaternion.LookRotation(currenTarget.position - _targetPosition);

            // IMPORTANT: Update private _pitch variable
            // This keeps our internal record in sync with the new angle so the camera won't break when we rotate again
            _pitch = transform.eulerAngles.x;
        }
    }

    // Movement Logic
    // Now we need to tell the camera what to do with the data we just collected. 

    // Public void HandleOrbiting() will be called in the LateUpdate method, and it will use the _mouseDelta and _isOrbiting variables to determine how to rotate the camera
    // When we right click, we want to change the camera's Euler Angles
    // Mouse X will change the Y rotation (looking left and right)
    // Mouse Y will change the X rotation (looking up and down)
    public void HandleOrbiting()
    {
        if (_isOrbiting)
        {
            // We update our 'internal record' of the pitch.
            // We subtract because in most games, moving mouse UP should decrease the X-angle.
            _pitch -= _mouseDelta.y * _rotationSensitivity_Y;

            // We clamp the pitch to prevent the camera from flipping upside down
            // We stop the internal record from going past looking straight up (90 degrees) or straight down (-90 degrees)
            _pitch = Mathf.Clamp(_pitch, -90f, 90f);

            // We apply the Clean record to the X, but we can still read the Y ( yaw) from the transform
            // because rotating horizontally doesn't cause any issues with gimbal lock. We only need to worry about clamping the pitch to prevent flipping.
            transform.rotation = Quaternion.Euler(_pitch, transform.eulerAngles.y + (_mouseDelta.x * _rotationSensitivity_X), 0);
        }
    }

    // Public void HandlePanning() We want sliding relative to where the camera is looking
    // If you just add transform.position, the camera will always slide along the world's X/Y grid, we want to avoid that.
    // We then use transform.TransformDirection() to convert Left/Up into the camera's perspective.
    public void HandlePanning()
    {
        if (_isPanning)
        {
            // We create a Vector3 based on the mouse movement, with the Z value set to 0 because we don't want to move forward/backward when panning
            Vector3 panMove = new Vector3(-_mouseDelta.x, -_mouseDelta.y, 0) * _panningSensitivity;

            // Add to target, let Lerp handle the actual movement
            _targetPosition += transform.TransformDirection(panMove);
        }
    }

    // Public void HandleZooming() We want to move forward and backward along the camera's forward axis based on the scroll wheel input
    public void HandleZooming()
    {
        // We add a small threshold to prevent jittering when the scroll wheel is at rest
        if (Mathf.Abs(_scrollInput) > 0.01f)
        {
            // Calculate the direction based on where the camera is facing, and the scroll input
            Vector3 zoomDir = transform.forward * _scrollInput * _zoomSensitivity;

            // We add that to our desired position, not to the actual transform 
            _targetPosition += zoomDir;
        }
    }

    // Private void ApplySmoothMovement Smoothly interpolates the camera's current position toward the calculated target position using a linear blend.
    private void ApplySmoothMovement()
    {
        // LERP for smooth movement over time
        transform.position = Vector3.Lerp(transform.position, _targetPosition, lerpSpeed * Time.deltaTime);
    }
}
