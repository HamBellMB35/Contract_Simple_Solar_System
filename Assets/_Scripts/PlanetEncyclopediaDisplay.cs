using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlanetEncyclopediaDisplay : MonoBehaviour
{

    [Header("UI Text Fields")]
    [SerializeField] private TextMeshProUGUI planetNameField;
    [SerializeField] private TextMeshProUGUI massField;
    [SerializeField] private TextMeshProUGUI sizeField;
    [SerializeField] private TextMeshProUGUI rotationField;
    [SerializeField] private TextMeshProUGUI revolutionField;
    [SerializeField] private TextMeshProUGUI speedField;


    // This public method reads a data asset card and fills out our UI text fields
    public void DisplayPlanetInfo(PlanetData dataCard)
    {
        if (dataCard == null) return; // Safety check

        planetNameField.text = "Planet: " + dataCard.planetName;
        massField.text = dataCard.planetMass;
        sizeField.text = dataCard.planetSize;
        rotationField.text = dataCard.planetRotation;
        revolutionField.text = dataCard.planetRevolution;
        speedField.text = dataCard.orbitalSpeed;

    }



}
