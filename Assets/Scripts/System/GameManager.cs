using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void StateChangedHandler(GameState state);
    public event StateChangedHandler StateChanged;

    public enum GameState
    {
        MainMenu,
        Playing,
        Loading,
        Waiting,
        Paused,
        LevelComplete,
        GameOver,
    }

    public GameState State { get; private set; } = GameState.MainMenu;

    public float LoadingProgress { get; private set; }

    [SerializeField]
    [Tooltip("The number of seconds to wait before switching to the LevelComplete state.")]
    private float lvlCompleteWait = 5f;

    private const int MainMenuIdx = 0;
    private const int FirstLevelIdx = 1;

    public void TogglePause()
    {
        // Flip between Playing and Paused
        if (State == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
        else if (State == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }

    public void StartGame()
    {
        // Load Level 1
        LoadScene(FirstLevelIdx);
    }

    public void RestartLevel()
    {
        // Get the current level index and reload that level
        int currentLevelIdx = SceneManager.GetActiveScene().buildIndex;
        LoadScene(currentLevelIdx);
    }

    public void NextLevel()
    {
        // Get the next level index
        int nextLevelIdx = SceneManager.GetActiveScene().buildIndex + 1;

        // If we've completed the game, go back to Main Menu
        if (nextLevelIdx >= SceneManager.sceneCountInBuildSettings)
        {
            QuitToMenu();
            return;
        }

        // Otherwise load the next level
        LoadScene(nextLevelIdx);
    }

    public void QuitToMenu()
    {
        // Load Main Menu
        LoadScene(MainMenuIdx);
    }

    private void SetState(GameState newState)
    {
        if (newState == State)
        {
            return;
        }

        State = newState;

        // Freeze gameplay while paused
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;

        StateChanged?.Invoke(newState);
    }

    public void LevelComplete()
    {
        SetState(GameState.Waiting);
        StartCoroutine(CompleteAfterSeconds(lvlCompleteWait));
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    private void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadSceneAsync(buildIndex));
    }

    private IEnumerator LoadSceneAsync(int buildIndex)
    {
        LoadingProgress = 0f;
        SetState(GameState.Loading);

        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);

        while (!operation.isDone)
        {
            LoadingProgress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        LoadingProgress = 1f;
        SetState(buildIndex == MainMenuIdx ? GameState.MainMenu : GameState.Playing);
    }

    private IEnumerator CompleteAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetState(GameState.LevelComplete);
    }
}
