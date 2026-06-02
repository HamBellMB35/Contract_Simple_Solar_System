using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Planet Data", menuName = "Solar System/Planet Data")]
public class PlanetData : ScriptableObject
{
    [Header("Core Identification")]
    public string planetName;

    [Header("Physical Characteristics")]
    public string planetMass;        // Storing as string lets us use scientific notation like 5.97 x 10^24 kg
    public string planetSize;        // Storing as string lets us use scientific notation like 5.97 x 10^24 kg
    public string planetRotation;    // e.g., "24 Hours"
    public string planetRevolution;  // e.g., "365 Days"
    public string orbitalSpeed;      // e.g., "29.78 km/s"

    [Header("Encyclopedia Text")]
    [TextArea(3, 10)] // Gives us a nice big paragraph text box in the Inspector
    public string planetDescription;

}
