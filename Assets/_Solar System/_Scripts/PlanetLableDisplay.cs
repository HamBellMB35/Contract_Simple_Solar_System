using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetLableDisplay : MonoBehaviour
{
    [Tooltip("This is the planet with a lable; Drag the planet here")]
    [SerializeField] private Transform _planetTransform;

    [Tooltip("This is the position of the lable relative to the planet")]
    [SerializeField] private float _dynamicOffset;

    [Tooltip("This is the extra percentage of breathing room for the lable's height")]
    [SerializeField] private float _breathingRoom;

    private Camera _mainCamera;
    private TextMeshProUGUI _textComponent;



    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // DisplayLable();


        PositionLabel();

    }

  
    //private void DisplayLable()
    //{
    //    if (_planetTransform == null || _mainCamera == null) return;
    //    
    //    Position the lable above the planet
    //    transform.position = _planetTransform.position + dynamicOffset;
    //    
    //    Make the lable face the camera
    //    //transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
    //    //                 _mainCamera.transform.rotation * Vector3.up);

    //    // Billboard effect: Align the lable's rotation with the camera's rotation
    //    transform.rotation = _mainCamera.transform.rotation;
    //}

    public void SetTarget(Transform target, string planetName)
    {
        _planetTransform = target;

        // Get the text component and set the planet name
        _textComponent = GetComponent<TextMeshProUGUI>();
        if (_textComponent != null)
        {
            _textComponent.text = planetName;
        }

        // Find the "height" of the planet by checking its collider bounds
        // This works even if the planet is scaled up or down
        if (target.TryGetComponent<Renderer>(out Renderer ren))
        {
            // Extents.y is half the height of the planet. We add a little extra (10%) for breathing room
            _dynamicOffset = ren.bounds.extents.y  + _breathingRoom;

        }

    }

    private void PositionLabel()
    {
        if (_planetTransform == null) return;

        // Position the label at the planet's center + its own specific radius
        transform.position = _planetTransform.position + new Vector3(0, _dynamicOffset, 0);

        // Standard Billboard rotation
        transform.rotation = _mainCamera.transform.rotation;
    }

   
}
