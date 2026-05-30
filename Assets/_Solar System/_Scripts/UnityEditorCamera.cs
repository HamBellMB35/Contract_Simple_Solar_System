using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityEditorCamera : MonoBehaviour
{
    // These store raw data from the mouse and keyboard inputs
    private Vector2 _mouseDelta;
    private Vector2 _movementInput;
    private float _verticalInput;
    private float _scrollInput;
    private float _pitch;
    private Vector3 _targetPosition;

    // This caches our ongoing flight velocity vector to give keyboard controls smooth acceleration and friction drag
    private Vector3 _smoothedFlightVelocity; // ADDED: Blends our flight vector over time for smooth momentum

    // These store the YES/NO states of the buttons
    private bool _isOrbiting;
    private bool _isPanning;
    private bool _isFollowing;

    // This holds the calculated physical radius size of whatever planet we are currently focusing on
    private float _currentPlanetRadius;

    // Sensitivity settings for the camera movement
    [Header("Mouse sensitivity settings")]
    [SerializeField] private float _rotationSensitivity_X = 0.1f;
    [SerializeField] private float _rotationSensitivity_Y = 0.1f;
    [SerializeField] private float _panningSensitivity = 0.1f;
    [SerializeField] private float _zoomSensitivity = 0.05f;

    [Header("Keyboard Flight Settings")]
    [SerializeField] private float _flightSpeed = 20f; // ADDED

    [Tooltip("This are the camera smoothing controls")]
    [SerializeField] private float lerpSpeed = 5f;

    [Header("Focus settings")]
    [SerializeField] private float _focusDistance = 15f;
    [SerializeField] private float _anchorDistanceStartClamp = 1f;
    [SerializeField] private float _anchorDistanceLimitClamp = 100f; // Bumped this up slightly so you can scroll back further in space
    [SerializeField] private float _followDistanceAnchor; // So the follow distance doesnt get overwritten by lerp? << REVISIT THIS LATER >>
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
        HandleFollowing();
        HandleKeyboardFlight();

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
        _scrollInput = context.ReadValue<float>();
    }

    public void OnFlyVertical(InputAction.CallbackContext context)
    {
        // We read the vertical input for ascending and descending
        _verticalInput = context.ReadValue<float>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // ReadValue tells the script to treat the WASD keyboard composite layout as a 2D Vector2 coordinate
        // X will store Left/Right (-1 to 1) and Y will store Forward/Backward (-1 to 1)
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        // Only execute when the button is first tapped down ('performed' phase)
        if (context.performed)
        {
            // Instead of running the heavy reflection code inside the raw callback, 
            // we safely route the input action trigger right into our dedicated execution method below
            ExecuteFocus();
        }
    }

    // This isolated method handles the raw target framing calculations directly.
    // By keeping it separate from the Input System callback structure, external scripts (like our Search Bar)
    // can call it instantly without forcing Unity to allocate background frame memory or throw performance stalls!
    public void ExecuteFocus()
    {
        if (currenTarget != null)
        {
            // We tell the script we are offcially following a target
            _isFollowing = true;

            // We find the direction from the planet to the camera
            Vector3 directionToCamera = (transform.position - currenTarget.position).normalized;

            // Safety Check: If the camera is exactly on top of the planet, back up along the z axis
            if (directionToCamera == Vector3.zero)
            {
                directionToCamera = Vector3.back;
            }

            // DYNAMIC RADIUS CALCULATION: We look for a renderer in the target or its children to find its physical size bounds.
            // This reads the actual mesh data once upon click so it doesn't hurt performance inside Update loops!
            _currentPlanetRadius = 0f;
            Renderer planetRenderer = currenTarget.GetComponentInChildren<Renderer>();
            if (planetRenderer != null)
            {
                // Extents.y gives us exactly half the visual height of the planet mesh shape (its radius)
                _currentPlanetRadius = planetRenderer.bounds.extents.y;
            }

            // FOLLOW FIX: We calculate our absolute goal tracking distance by pairing the target's physical size with our focus padding
            // We save this number in our anchor box so it cant be modified or lost during the lerp process
            _followDistanceAnchor = _currentPlanetRadius + _focusDistance;

            // We set our phantom target position to sit outside the planet using our new clear anchor calculation
            _targetPosition = currenTarget.position + (directionToCamera * _followDistanceAnchor);

            // OPTIMIZATION FIX 2: Manually synchronize the physical camera position right at the start frame of focus.
            // This prevents the smooth movement interpolation loop from dropping frames or rubber-banding.
            transform.position = _targetPosition;

            // We want the camera to look directly at the planet's center 
            // We calculate this from our intended target position so our view angles stay mathematically synchronized
            transform.rotation = Quaternion.LookRotation(currenTarget.position - _targetPosition);

            // IMPORTANT: Update private _pitch variable
            // This keeps our internal record in sync with the new angle so the camera won't break when we rotate again
            _pitch = transform.eulerAngles.x;
        }
    }

    public void OnUnfocus(InputAction.CallbackContext context)
    {
        // Only execute when the button is first tapped down ('performed' phase)
        if (context.performed)
        {
            _isFollowing = false;
            Debug.Log("Camera follow disabled via Escape key.");
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

            // BREAKAWAY:  If we start orbiting, we are no longer following the target, we are now in free-look mode
            _isFollowing = false;
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
            // Add to target, let Lerp handle the actual movement
            Vector3 panMove = new Vector3(-_mouseDelta.x, -_mouseDelta.y, 0) * _panningSensitivity;
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

            // FOLLOW FIX 2: If we are currently following a target, we need to update our follow distance anchor based on the zoom input, so the camera doesn't fight us when we try to zoom in and out while following
            if (_isFollowing)
            {
                // ZOOM JITTER FIX: We directly scale our distance tracking gap in alignment with the incoming scroll numbers.
                _followDistanceAnchor -= _scrollInput * _zoomSensitivity;

                // AUTOMATED BOUNDARY FIX: Instead of checking a blind variable in the inspector, we dynamically ensure
                // our lower limit can never cross inside the physical radius boundary wall of the target celestial body mesh!
                // We add a 2 unit safety cushion so you stay cleanly floating right outside the mesh surface textures.
                float safeMinimumZoom = _currentPlanetRadius + 2f;
                _followDistanceAnchor = Mathf.Clamp(_followDistanceAnchor, safeMinimumZoom, _anchorDistanceLimitClamp);
            }
        }
    }

    private void HandleFollowing()
    {
        if (_isFollowing && currenTarget != null)
        {
            // Unity's built in LookAt function is really useful for this, it handles all the math of rotating to face the target for us
            // it forces the camer's Z forward axis to point directly at the target's center coordinates every single fram, and the Y axis to stay upright
            transform.LookAt(currenTarget.position, Vector3.up);
        }
    }

    private void HandleKeyboardFlight()
    {
        // We calculate relative directional pathways based on the exact angle the camera lens is currently looking
        Vector3 forwardFlight = transform.forward * _movementInput.y;
        Vector3 sidewaysFlight = transform.right * _movementInput.x;
        Vector3 verticalFlight = transform.up * _verticalInput;

        // We combine those directional pathways into a single flight vector
        Vector3 targetDirection = (forwardFlight + sidewaysFlight + verticalFlight);

        // If any key is pressed, we scale the vector down to prevent diagonal acceleration
        if (targetDirection.sqrMagnitude > 0.01f)
        {
            targetDirection = targetDirection.normalized;

            // BREAKAWAY: If the player manually orverrrides systems poistions by typing movement, we turn the following off and go into free-flight mode
            _isFollowing = false;

            // We gradually slide our current flight velocity vector toward the  desired target vector
            // This handles smooth acceleration up to full speed and clean deceleration drifting back down to rest when released
            _smoothedFlightVelocity = Vector3.Lerp(_smoothedFlightVelocity, targetDirection * _flightSpeed, Time.unscaledDeltaTime * lerpSpeed);

            // Now we append our smoothed flight velocity tracking offsets directly onto our phantom target destination tracker coordinates
            _targetPosition += _smoothedFlightVelocity * Time.unscaledDeltaTime;
        }
    }

    // Private void ApplySmoothMovement Smoothly interpolates the camera's current position toward the calculated target position using a linear blend.
    private void ApplySmoothMovement()
    {
        if (_isFollowing && currenTarget != null)
        {
            // We find the compass direction poiting from the planet to the camera
            Vector3 directionFromPlanet = (transform.position - currenTarget.position).normalized;

            // FOLLOW FIX 3: Instead of calculating distance from a laggin camera, we use the solid fixed anchor distance value
            _targetPosition = currenTarget.position + (directionFromPlanet * _followDistanceAnchor);
        }

        // LERP for smooth movement over time
        transform.position = Vector3.Lerp(transform.position, _targetPosition, lerpSpeed * Time.unscaledDeltaTime);
    }
}