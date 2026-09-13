using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The persistent GameManager that handles game state changes and scene loading")]
    private GameManager gameManager;

    [SerializeField]
    [Tooltip("The AudioSource that plays looping menu and gameplay music.")]
    private AudioSource music;

    [SerializeField]
    [Tooltip("The AudioSource that plays one-shot sound effects.")]
    private AudioSource soundEffects;

    [SerializeField]
    [Tooltip("The AudioSource that plays looping backround noise during gameplay.")]
    private AudioSource ambience;

    [Header("Music and Ambience Clips")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The music clip to play during gameplay.")]
    private AudioClip gameplayClip;

    [SerializeField]
    [Tooltip(
        "The music clip to play in menus such as Main Menu, Pause Menu, and end-of-level screens."
    )]
    private AudioClip menuClip;

    [SerializeField]
    [Tooltip("The background noise to play during gameplay.")]
    private AudioClip ambienceClip;

    [Header("Sound Effect Clips")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The clip to play when a menu button is clicked.")]
    private AudioClip buttonClickClip;

    void Start()
    {
        gameManager.StateChanged += StateChangedHandler;
        ambience.clip = ambienceClip;

        // Get the GameManager state on first load
        StateChangedHandler(gameManager.State);
    }

    private void StateChangedHandler(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.MainMenu:
            {
                SetAmbience(false);
                PlayMusic(menuClip);
                break;
            }
            case GameManager.GameState.Playing:
            {
                SetAmbience(true);
                PlayMusic(gameplayClip);
                break;
            }
            case GameManager.GameState.Paused:
            {
                SetAmbience(false);
                PlayMusic(menuClip);
                break;
            }
            case GameManager.GameState.LevelComplete:
            {
                SetAmbience(false);
                PlayMusic(menuClip);
                break;
            }
            case GameManager.GameState.GameOver:
            {
                SetAmbience(false);
                PlayMusic(menuClip);
                break;
            }
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (music.clip == clip)
        {
            return;
        }

        music.clip = clip;
        music.Play();
    }

    private void SetAmbience(bool enabled)
    {
        if (enabled)
        {
            if (!ambience.isPlaying)
            {
                ambience.Play();
            }
        }
        else
        {
            ambience.Stop();
        }
    }

    public void PlayButtonClick()
    {
        soundEffects.PlayOneShot(buttonClickClip);
    }
}
