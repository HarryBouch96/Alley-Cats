using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public delegate void CatSpawnedHandler(CatController cat);
    public event CatSpawnedHandler CatSpawned;

    [Header("References")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The cat prefab instantiated once and reused for each shot.")]
    private CatController catPrefab;

    [SerializeField]
    [Tooltip("The slingshot for this level.")]
    private SlingshotController slingshot;

    [SerializeField]
    [Tooltip("The golden dumpster for this level.")]
    private DumpsterController dumpster;

    [SerializeField]
    [Tooltip("The level bounds collider.")]
    private BoxCollider2D levelBounds;

    [Header("Level settings")]
    [Space(10)]
    [SerializeField]
    [Tooltip(
        "The number of cats the player should start with on this level (must be equal to maxScore + 1)."
    )]
    private int maxCatCount = 4;

    [SerializeField]
    [Tooltip("The maximum score achievable")]
    private int maxScore = 3;

    private CatController activeCat;
    private CamControl camControl;
    private GameManager gameManager;
    private UIManager uiManager;
    private int currentCatCount;

    [SerializeField]
    [Tooltip("The minimum of number of shots to complete the level")]
    private int minShots = 2;

    private void Start()
    {
        currentCatCount = maxCatCount;

        gameManager = FindFirstObjectByType<GameManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        uiManager.SetCatCount(currentCatCount);
        uiManager.ClearScore();

        // Create one cat and reuse it for every attempt in this level
        activeCat = Instantiate(catPrefab);
        activeCat.SetLevelBounds(levelBounds.bounds);
        activeCat.ShotFinished += ShotFinishedHandler;

        dumpster.LevelComplete += LevelCompleteHandler;

        camControl = Camera.main.GetComponent<CamControl>();
        camControl.SetTarget(activeCat.transform);

        slingshot.LoadCat(activeCat);
        CatSpawned?.Invoke(activeCat);
    }

    private void ShotFinishedHandler(CatController cat)
    {
        if (gameManager.State == GameManager.GameState.Playing)
        {
            currentCatCount--;
            uiManager.SetCatCount(currentCatCount);

            if (currentCatCount > 0)
            {
                slingshot.LoadCat(activeCat);
            }
            else
            {
                gameManager.GameOver();
            }
        }
    }

    private void LevelCompleteHandler()
    {
        if (gameManager.State == GameManager.GameState.Playing)
        {
            int score = CalculateScore();
            uiManager.SetScore(score, maxScore);
            gameManager.LevelComplete();
        }
    }

    private int CalculateScore()
    {
        // + 1 because the active cat is still in play until it stops moving
        float shotsUsed = maxCatCount - currentCatCount + 1;

        // The number of shots taken above the minimum required
        float wastedShots = shotsUsed - minShots;

        // The most wasted shots while still winning
        float worstCase = maxCatCount - minShots;

        if (worstCase == 0f)
        {
            return maxScore;
        }

        return (int)Math.Round(maxScore - wastedShots / worstCase * (maxScore - 1));
    }
}
