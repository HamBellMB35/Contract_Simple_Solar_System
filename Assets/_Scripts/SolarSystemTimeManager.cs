using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemTimeManager : MonoBehaviour
{
    [SerializeField] private float _buttonShrinkScale = 0.85f; // Scale factor for shrinking the button


    // This public method will turn time to 0, freezing all planet rotations
    public void PauseSimulation()
    {
        Time.timeScale = 0f;
        Debug.Log("Simulation paused.");
    }

    public void PlaySimulation()
    {
        Time.timeScale = 1f;
        Debug.Log("Simulation playing.");
    }

    // This public method is an entry point for our buttons
    // It accepts a transform parameter so it knows exactly which button was clicked, and can react accordingly

    public void AnimateButton(Transform buttonTransform)
    {
        if(buttonTransform != null)
        {

            // Start background timer routine
            StartCoroutine(ModifyScaleRoutine(buttonTransform));


        }
    }

    private IEnumerator ModifyScaleRoutine(Transform buttonTransform)
    {
        // We Store the original scale of the button so we can return to it later
        Vector3 originalScale = buttonTransform.localScale;


        // We shrink the buttton down to the specified scale factor
        buttonTransform.localScale = originalScale * _buttonShrinkScale;


        // WAIT FOR 0.5 SECONDS
        // CRITICAL CHECK: We use WaitForSecondsRealtime instead of WaitForSeconds.
        // Because the pause button sets Time.timeScale to 0, standard time waiting freezes! 
        // Realtime looks at your physical wristwatch, ensuring the animation still plays while paused
        yield return new WaitForSecondsRealtime(0.25f);

        // We return the button to its original scale
        buttonTransform.localScale = originalScale;


    }

    // This public method will close the application when hooked up to a UI button
    public void ExitSimulation()
    {
        Debug.Log("Exiting Solar System Simulation...");

        // 1. If we are running inside the Unity Editor, stop the play mode
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // 2. If we are running a built executable game, close the application window
        Application.Quit();
    }

}
