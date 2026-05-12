using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{

    [Header("Planet Settings")]
    [SerializeField] private GameObject _labelPrefab;
    [SerializeField] private Transform _globalCanvasTransform;

    [Header("Planet Configuration Data")]
    [SerializeField] private GameObject[] _planets;
    [SerializeField] private string[] _planetNames;


    void Start()
    {
        // Safety check to ensure the arrays are of the same length
        if(_planets.Length != _planetNames.Length)
        {
            Debug.LogWarning("Planets and Planet Names arrays must be of the same length.");

        }

        // Loop using index (i) to match planets with their names
        for (int i = 0; i < _planetNames.Length; i++)
        {
            if (_planets[i] == null)
            {
                Debug.LogWarning($"Planet at index {i} is not assigned.");
                continue;
            }

            // Create a label
            GameObject newLabel = Instantiate(_labelPrefab, _globalCanvasTransform);

            // Pass BOTH the transform and the name from our arrays to the label's script
            // We use [i] to access the item at the current position in the list
            PlanetLableDisplay labelScript = newLabel.GetComponent<PlanetLableDisplay>();

            // Check if we have a name for this index, ortherwise we use a placeHolder
            string nameToUse = (i < _planetNames.Length) ? _planetNames[i] : "Unknown Body";

            labelScript.SetTarget(_planets[i].transform, nameToUse);
        }

    }

  
}
