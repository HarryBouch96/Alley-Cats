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

    [SerializeField]
    private GameObject lvlCompleteScreen;

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private GameObject catIconPrefab;

    [SerializeField]
    private GameObject fishIconPrefab;

    [SerializeField]
    private GameObject noFishIconPrefab;

    private Transform livesContainer;
    private Transform scoreContainer;

    // Called by the LevelManager when the number of cats remaining changes
    public void SetCatCount(int count)
    {
        // Add a cat icon for each cat if we haven't already
        while (livesContainer.childCount < count)
        {
            // False sets the icon's position relative to its parent
            Instantiate(catIconPrefab, livesContainer, false);
        }

        // Set the first `catCount` icons active and the remaining ones inactive
        for (int i = 0; i < livesContainer.childCount; i++)
        {
            livesContainer.GetChild(i).gameObject.SetActive(i < count);
        }
    }

    public void SetScore(int score, int maxScore)
    {
        for (int i = 0; i < maxScore; i++)
        {
            GameObject prefab = i < score ? fishIconPrefab : noFishIconPrefab;
            Instantiate(prefab, scoreContainer, false);
        }
    }

    public void ClearScore()
    {
        for (int i = 0; i < scoreContainer.childCount; i++)
        {
            Destroy(scoreContainer.GetChild(i).gameObject);
        }
    }

    private void Start()
    {
        livesContainer = hud.transform.Find("Lives");
        scoreContainer = lvlCompleteScreen.transform.Find("Score");

        // Subscribe to GameManager state changes
        gameManager.StateChanged += StateChangedHandler;

        // Get the GameManager state on first load
        StateChangedHandler(gameManager.State);
    }

    private void StateChangedHandler(GameManager.GameState state)
    {
        mainMenu.SetActive(state == GameManager.GameState.MainMenu);
        pauseMenu.SetActive(state == GameManager.GameState.Paused);
        gameOverScreen.SetActive(state == GameManager.GameState.GameOver);
        lvlCompleteScreen.SetActive(state == GameManager.GameState.LevelComplete);

        // In-game HUD remains active behind Pause Menu and end of level screens
        hud.SetActive(state != GameManager.GameState.MainMenu);
    }
}
