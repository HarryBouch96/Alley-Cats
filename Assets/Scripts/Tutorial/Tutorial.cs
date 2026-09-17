using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Header("References")]
    [Space(10)]
    [Tooltip("The LevelManager for this level.")]
    [SerializeField]
    private LevelManager lvlManager;

    [Tooltip("The slingshot for this level.")]
    [SerializeField]
    private SlingshotController slingshot;

    [Tooltip("The Animator component of the tutorial icon.")]
    [SerializeField]
    private Animator animator;

    private GameManager gameManager;
    private CatController cat;

    // Subscribe to LevelManager before it spawns a cat
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        lvlManager.CatSpawned += SubscribeToCat;
        slingshot.AimingStarted += () => animator.SetBool("hasAimed", true);
    }

    private void SubscribeToCat(CatController spawnedCat)
    {
        cat = spawnedCat;
        spawnedCat.ReachedApex += ShowPounceAnim;
        spawnedCat.Pounced += HidePounceAnim;
    }

    private void ShowPounceAnim()
    {
        cat.ReachedApex -= ShowPounceAnim;
        gameManager.SetTimeFrozen(true);
        animator.SetBool("hasReachedApex", true);
    }

    private void HidePounceAnim()
    {
        cat.ReachedApex -= ShowPounceAnim;
        cat.Pounced -= HidePounceAnim;
        gameManager.SetTimeFrozen(false);
        animator.SetBool("hasPounced", true);
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(cat.transform.position);
    }
}
