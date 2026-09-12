using UnityEngine;

public class LevelManager : MonoBehaviour
{
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

    [Header("Level settings")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The number of cats the player should start with on this level.")]
    private int catCount = 3;

    private CatController activeCat;
    private CamControl camControl;
    private GameManager gameManager;
    private UIManager uiManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        uiManager.SetCatCount(catCount);

        // Create one cat and reuse it for every attempt in this level
        activeCat = Instantiate(catPrefab);
        activeCat.ShotFinished += ShotFinishedHandler;

        dumpster.LevelComplete += LevelCompleteHandler;

        camControl = Camera.main.GetComponent<CamControl>();
        camControl.SetTarget(activeCat.transform);

        slingshot.LoadCat(activeCat);
    }

    private void ShotFinishedHandler(CatController cat)
    {
        if (gameManager.State == GameManager.GameState.Playing)
        {
            catCount--;
            uiManager.SetCatCount(catCount);

            if (catCount > 0)
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
            gameManager.LevelComplete();
        }
    }
}
