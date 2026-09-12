using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("GameManager")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The persistent GameManager that handles game state changes and scene loading")]
    private GameManager gameManager;

    [Header("Menus, screens, and HUD")]
    [Space(10)]
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject hud;

    [SerializeField]
    private GameObject pauseMenu;

    // [SerializeField]
    // private GameObject levelCompleteScreen;

    // [SerializeField]
    // private GameObject gameOverScreen;

    [SerializeField]
    private GameObject catIconPrefab;

    private Transform livesContainer;

    // Called by the LevelManager when the number of cats remaining changes
    public void SetCatCount(int catCount)
    {
        // Add an icon for each cat if it doesn't exist yet
        while (livesContainer.childCount < catCount)
        {
            // False sets the icon's position relative to its parent
            Instantiate(catIconPrefab, livesContainer, false);
        }

        // Set the first `catCount` icons active and the remaining ones inactive
        for (int i = 0; i < livesContainer.childCount; i++)
        {
            livesContainer.GetChild(i).gameObject.SetActive(i < catCount);
        }
    }

    private void Start()
    {
        livesContainer = hud.transform.Find("Lives");

        // Subscribe to GameManager state changes
        gameManager.StateChanged += StateChangedHandler;

        // Get the GameManager state on first load
        StateChangedHandler(gameManager.State);
    }

    private void StateChangedHandler(GameManager.GameState state)
    {
        mainMenu.SetActive(state == GameManager.GameState.MainMenu);
        pauseMenu.SetActive(state == GameManager.GameState.Paused);
        // levelCompleteScreen.SetActive(state == GameManager.GameState.LevelComplete);
        // gameOverScreen.SetActive(state == GameManager.GameState.GameOver);

        // In-game HUD remains active behind Pause Menu and end of level screens
        hud.SetActive(state != GameManager.GameState.MainMenu);
    }
}
