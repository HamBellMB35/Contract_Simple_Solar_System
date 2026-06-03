using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpaceEncyclopediaUIDriver : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CameraInputRouter inputRouter;
    [SerializeField] private CameraFlightMotor cameraMotor;

    [Header("Encyclopedia Integration")]
    [Tooltip("Drag your InforPromtText GameObject here!")]
    [SerializeField] private GameObject _infoPromptText;

    [Header("Encyclopedia Panel Hook")]
    [SerializeField] private GameObject _encyclopediaPanel;
    [SerializeField] private PlanetEncyclopediaDisplay _encyclopediaDisplay;

    // Stores the YES/NO state of whether our main data panel layout overlay is open on screen
    private bool _isDataPanelOpen = false;

    // Public getter pattern property so the camera flight motor can check if it needs to shift left/right
    public bool IsDataPanelOpen => _isDataPanelOpen;

    private void OnEnable()
    {
        // We link into our central input listener system tracks when the object wakes up
        if (inputRouter != null)
        {
            inputRouter.OnOpenEncyclopediaPerformed += HandleToggleEncyclopedia;
            inputRouter.OnUnfocusPerformed += ForceCloseUI;
        }
    }

    private void OnDisable()
    {
        // Cleanup safety: Unhook our listeners when disabled to prevent frame memory drops!
        if (inputRouter != null)
        {
            inputRouter.OnOpenEncyclopediaPerformed -= HandleToggleEncyclopedia;
            inputRouter.OnUnfocusPerformed -= ForceCloseUI;
        }
    }

    // Public caller utility so external engines can explicitly wake up or flip screen text boxes
    public void SetPromptVisibility(bool isVisible)
    {
        if (_infoPromptText != null)
        {
            _infoPromptText.SetActive(isVisible);
        }
    }

    // **** FIX: We close the data panel if its open when we click to focus on a new planet or fly away, 
    // this prevents the bug where the data panel gets stuck open with no data when you look away
    public void ForceCloseUI()
    {
        _isDataPanelOpen = false;

        if (_encyclopediaPanel != null) _encyclopediaPanel.SetActive(false);
        if (_infoPromptText != null) _infoPromptText.SetActive(false);
    }

    private void HandleToggleEncyclopedia()
    {
        // Safety guard check: Only run if we are currently tracking a valid planet data card inside the flight computer
        if (cameraMotor == null || cameraMotor.hoveredPlanetData == null) return;

        _isDataPanelOpen = !_isDataPanelOpen; // Toggle the data panel on and off with the same key

        if (_encyclopediaPanel != null)
        {
            _encyclopediaPanel.SetActive(_isDataPanelOpen);
        }

        // === NEW FIX: Handle the "Press I for info" prompt text visibility ===
        if (_infoPromptText != null)
        {
            // If the big panel is open (!True = False), hide the prompt text.
            // If the big panel is closed (!False = True), show the prompt text.
            _infoPromptText.SetActive(!_isDataPanelOpen);
        }

        if (_isDataPanelOpen && _encyclopediaPanel != null)
        {
            // We Pass our outomated data card over to our display window text fields
            _encyclopediaDisplay.DisplayPlanetInfo(cameraMotor.hoveredPlanetData);
        }
    }
}
