using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SlingshotController : MonoBehaviour
{
    public delegate void AimingStartedHandler();
    public event AimingStartedHandler AimingStarted;

    [Header("References")]
    [Space(10)]
    [SerializeField]
    [Tooltip(
        "The transform component of the Launch Point game object from which the cat is launched."
    )]
    private Transform launchPoint;

    [SerializeField]
    [Tooltip(
        "The transform component of the Left Band Anchor game object that the start of the rubber band is attached to."
    )]
    private Transform leftBandAnchor;

    [SerializeField]
    [Tooltip(
        "The transform component of the Left Band Anchor game object that the end of the rubber band is attached to."
    )]
    private Transform rightBandAnchor;

    [SerializeField]
    [Tooltip("The audio clip to play when stretching the slingshot band to aim.")]
    private AudioClip aimAudioClip;

    [SerializeField]
    [Tooltip("The audio clip to play when the slingshot band is released.")]
    private AudioClip releaseAudioClip;

    [Header("Launch settings")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The maximum distance the rubber band can be stretched from the launch point.")]
    private float maxDragDistance = 3f;

    [SerializeField]
    [Tooltip("The minimum height the rubber band can be pulled down to.")]
    private float minDragHeight = 0.5f;

    [SerializeField]
    [Tooltip("The amount of force applied to the cat per unit of drag distance.")]
    private float slingshotPower = 10f;

    private Camera cam;
    private GameManager gameManager;
    private CatController loadedCat;
    private LineRenderer dragLine;
    private AudioSource audioSource;
    private bool hasLaunched;
    private bool isAiming = false;

    private void Start()
    {
        cam = Camera.main;
        dragLine = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Disable the line until the player begins dragging
        dragLine.enabled = false;

        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        // Disable aiming while game is paused
        if (gameManager.State != GameManager.GameState.Playing)
        {
            dragLine.enabled = false;
            return;
        }

        // No cat loaded or cat has already been launched
        if (loadedCat == null || hasLaunched)
        {
            return;
        }

        // Check if player is trying to press the pause button
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Player has begun dragging the cat
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragLine.enabled = true;
        }

        // Player is still dragging the cat
        if (Mouse.current.leftButton.isPressed)
        {
            Aim();
        }

        // Player has released the cat
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Launch();
        }
    }

    private void Aim()
    {
        if (!isAiming)
        {
            isAiming = true;
            AimingStarted?.Invoke();
            audioSource.PlayOneShot(aimAudioClip);
        }

        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 offset = mousePosition - launchPoint.position;

        // Limit how far the cat can be dragged from the launch point
        offset = Vector3.ClampMagnitude(offset, maxDragDistance);

        Vector3 targetPosition = launchPoint.position + offset;
        targetPosition.z = launchPoint.position.z;
        targetPosition.y = Mathf.Max(targetPosition.y, transform.position.y + minDragHeight);

        loadedCat.SetAimPosition(targetPosition);

        dragLine.SetPosition(0, leftBandAnchor.position);
        dragLine.SetPosition(1, targetPosition);
        dragLine.SetPosition(2, rightBandAnchor.position);
    }

    private void Launch()
    {
        isAiming = false;
        audioSource.PlayOneShot(releaseAudioClip);

        hasLaunched = true;

        Vector3 dragVector = launchPoint.position - loadedCat.transform.position;

        loadedCat.Launch(dragVector * slingshotPower);
        dragLine.enabled = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            Mathf.Abs(cam.transform.position.z - launchPoint.position.z)
        );

        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPosition);
        worldPosition.z = launchPoint.position.z;

        return worldPosition;
    }

    public void LoadCat(CatController cat)
    {
        hasLaunched = false;
        isAiming = false;
        loadedCat = cat;
        loadedCat.ResetForShot(launchPoint.position);
    }
}
