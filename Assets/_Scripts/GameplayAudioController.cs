using UnityEngine;

public class GameplayAudioController : MonoBehaviour
{
    [Header("Audio Components")]
    [Tooltip("Drag your dedicated UI AudioSource here")]
    [SerializeField] private AudioSource uiAudioSource;

    [Header("Audio Clips")]
    [Tooltip("The standard tactile click sound effect")]
    [SerializeField] private AudioClip clickSoundClip;

    // A universal public method that any button click event in the scene can call
    public void PlayClickSound()
    {
        if (uiAudioSource != null && clickSoundClip != null)
        {
            // PlayOneShot prevents the sound from cutting itself off if clicked rapidly
            uiAudioSource.PlayOneShot(clickSoundClip);
        }
    }
}
