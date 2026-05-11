using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLableDisplay : MonoBehaviour
{
    [Tooltip("This is the planet with a lable; Drag the planet here")]
    [SerializeField] private Transform planetTransform;

    [Tooltip("This is the position of the lable relative to the planet")]
    [SerializeField] private Vector3 lableOffset = new Vector3(0, 5, 0);

    private Camera _mainCamera;


    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        DisplayLable();
    }

  
    private void DisplayLable()
    {
        if (planetTransform == null || _mainCamera == null) return;
        // Position the lable above the planet
        transform.position = planetTransform.position + lableOffset;
        // Make the lable face the camera
        //transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
        //                 _mainCamera.transform.rotation * Vector3.up);

        // Billboard effect: Align the lable's rotation with the camera's rotation
        transform.rotation = _mainCamera.transform.rotation;
    }
}
