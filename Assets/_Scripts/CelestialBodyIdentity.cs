using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyIdentity : MonoBehaviour
{
    [Header("Celestial Data Assignment")]
    [Tooltip("Drag the corresponding ScriptableObject data card asset file here)")]
    public PlanetData bodyData; // Holds the reference to this specific planet's stats blueprint

    // OnValidate runs automatically in the Unity Editor when scripts are loaded or values change
    private void OnValidate()
    {
        // Automation safety check: Only look for the data if the slow is currently empty
        if (bodyData == null)
        {
            // Unity searches the entire project asset directory for a ScriptableObject file 
            // matching the exact text name of this specific GameObject (e.g. "Earth")

            PlanetData foundData = Resources.Load<PlanetData>(gameObject.name + "Data");

            if (foundData != null)
            {
                bodyData = foundData;
                Debug.Log($"<color=green>SUCCESS:</color> Automatically linked {gameObject.name} to asset data card!");
            }

        }
    }


}
