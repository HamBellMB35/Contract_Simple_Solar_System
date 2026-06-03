using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;         

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Sub-Panels")]
    [Tooltip("Drag your overlay pop-up panels here!")]
    [SerializeField] private GameObject aboutPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Scene Configuration")]
    [Tooltip("The exact text name of your 3D solar system scene file")]
    [SerializeField] private string solarSystemSceneName = "SolarSystemScene";

    [Header("Audio Configurations")]
    [Tooltip("Drag the AudioSource component that is looping the background theme here")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [Tooltip("Drag the AudioSource dedicated to firing UI sound effects here")]
    [SerializeField] private AudioSource uiEffectsSource;
    [Tooltip("The click sound effect clip")]
    [SerializeField] private AudioClip clickSoundClip;

    [Header("Juice & Animation Settings")]
    [Tooltip("How small the button shrinks when clicked (e.g., 0.9 means 90% size)")]
    [SerializeField] private float shrinkScaleAmount = 0.9f;
    [Tooltip("How long the button stays shrunk before popping back up (in seconds)")]
    [SerializeField] private float punchDuration = 0.1f;

    private void Start()
    {
        // Ensure all extra informational screens are hidden on boot-up
        if (aboutPanel != null) aboutPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // Safety check: Make sure our background music source is actually playing and set to loop
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.loop = true;
            if (!backgroundMusicSource.isPlaying)
            {
                backgroundMusicSource.Play();
            }
        }
    }

    // Called by the PLAY button click event
    public void ClickPlaySimulation(RectTransform buttonTransform)
    {
        StartCoroutine(AnimateButtonPunch(buttonTransform, () =>
        {
            Debug.Log("Loading Solar System Simulation Universe...");

            // Before changing scenes, ensure Time.timeScale is back to normal!
            // If the player quit while paused, the global engine time might still be 0.
            Time.timeScale = 1f;

            SceneManager.LoadScene(solarSystemSceneName);
        }));
    }

    // Called by the ABOUT button
    public void ClickToggleAbout(RectTransform buttonTransform)
    {
        // We capture the current open/closed state of the panel before toggling it
        bool isCurrentlyOpen = aboutPanel != null && aboutPanel.activeSelf;

        StartCoroutine(AnimateButtonPunch(buttonTransform, () =>
        {
            if (aboutPanel != null) aboutPanel.SetActive(!isCurrentlyOpen);

            // Auto-close credits if about opens to prevent visual layering overlap bugs
            if (!isCurrentlyOpen && creditsPanel != null) creditsPanel.SetActive(false);
        }));
    }

    // Called by the CREDITS button
    public void ClickToggleCredits(RectTransform buttonTransform)
    {
        bool isCurrentlyOpen = creditsPanel != null && creditsPanel.activeSelf;

        StartCoroutine(AnimateButtonPunch(buttonTransform, () =>
        {
            if (creditsPanel != null) creditsPanel.SetActive(!isCurrentlyOpen);

            // Auto-close about if credits opens
            if (!isCurrentlyOpen && aboutPanel != null) aboutPanel.SetActive(false);
        }));
    }

    // Called by the QUIT simulation button
    public void ClickQuitSimulation(RectTransform buttonTransform)
    {
        StartCoroutine(AnimateButtonPunch(buttonTransform, () =>
        {
            Debug.Log("Exiting Solar System Simulation Application...");

            //  If running inside the Unity engine editor workspace, stop play mode playback
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            // If running a built standalone client application executable, close the game window
            Application.Quit();
        }));
    }

    // Universal Coroutine: Handles audio cues, scales down the rect transform matrix, pauses, and fires our code action sequence
    private IEnumerator AnimateButtonPunch(RectTransform targetButton, System.Action onCompleteAction)
    {
        if (targetButton == null)
        {
            // If something went wrong tracking the transform layout, run the logic action immediately and exit
            onCompleteAction?.Invoke();
            yield break;
        }

        // Audio Phase: Play the crisp click sound effect instantly
        if (uiEffectsSource != null && clickSoundClip != null)
        {
            uiEffectsSource.PlayOneShot(clickSoundClip);
        }

        // Animation Phase: Cache original transform dimensions and squeeze the button down
        Vector3 originalScale = targetButton.localScale;
        targetButton.localScale = originalScale * shrinkScaleAmount;

        // Pause Phase: Hold the shrink shape for a tiny fraction of a split-second
        yield return new WaitForSecondsRealtime(punchDuration);

        // Recovery Phase: Snap the button back up to its normal sizing dimensions
        targetButton.localScale = originalScale;

        // Execution Phase: Trigger the specific button method code blocks we queued up above
        onCompleteAction?.Invoke();
    }
}