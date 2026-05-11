using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    void Awake()
    {
        // Cache the camera reference for performance
        _mainCamera = Camera.main;
    }

    // LateUpdate is best for cameras to ensure the camera 
    // has finished moving before the text rotates to match it.
    void LateUpdate()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        if (_mainCamera == null) return;
        // Make the text face the camera
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                         _mainCamera.transform.rotation * Vector3.up);
    }
}
