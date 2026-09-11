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

    private enum LevelState
    {
        Playing,
        Complete,
        GameOver,
    }

    private LevelState state = LevelState.Playing;
    private CatController activeCat;
    private CamControl camControl;

    private void Start()
    {
        // Create one cat and reuse it for every attempt in this level
        activeCat = Instantiate(catPrefab);
        activeCat.ShotFinished += ShotFinishedHandler;

        dumpster.LevelComplete += LevelCompleteHandler;

        camControl = Camera.main.GetComponent<CamControl>();
        camControl.SetTarget(activeCat.transform);

        StartNewShot();
    }

    private void ShotFinishedHandler(CatController cat)
    {
        if (state == LevelState.Playing)
        {
            if (catCount > 0)
            {
                print("Starting a new shot! 🎯");
                StartNewShot();
            }
            else
            {
                state = LevelState.GameOver;
                print("No cats left! Game over!!! 😞");
            }
        }
    }

    private void LevelCompleteHandler()
    {
        if (state == LevelState.Playing)
        {
            state = LevelState.Complete;
            print("Level complete!!! 🏆");
        }
    }

    private void StartNewShot()
    {
        catCount--;
        slingshot.LoadCat(activeCat);
    }
}
