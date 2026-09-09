using UnityEngine;
using UnityEngine.InputSystem;

public class SlingshotController : MonoBehaviour
{
    [Header("References")]
    [Space(10)]
    [SerializeField]
    [Tooltip(
        "The transform component of the Launch Point game object from which the rubber band is drawn."
    )]
    Transform launchPoint;

    [SerializeField]
    [Tooltip(
        "The transform component of the Left Band Anchor game object that the start of the rubber band is attached to."
    )]
    Transform leftBandAnchor;

    [SerializeField]
    [Tooltip(
        "The transform component of the Left Band Anchor game object that the end of the rubber band is attached to."
    )]
    Transform rightBandAnchor;

    [SerializeField]
    [Tooltip("The rigidbody component of the Cat game object to be launched from the slingshot.")]
    Rigidbody catRb;

    [Header("Launch settings")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The maximum distance the rubber band can be stretched from the launch point.")]
    float maxDragDistance = 3f;

    [SerializeField]
    [Tooltip("The amount of force applied to the cat per unit of drag distance.")]
    float slingshotPower = 10f;

    private Camera mainCamera;
    private LineRenderer dragLine;
    private bool hasLaunched = false;

    private void Start()
    {
        mainCamera = Camera.main;
        dragLine = GetComponent<LineRenderer>();

        // Prevent physics from affecting the cat until it is launched
        catRb.isKinematic = true;

        // Disable the line until the player begins dragging
        dragLine.enabled = false;
    }

    private void Update()
    {
        // Player is not dragging or cat has already been launched
        if (Mouse.current == null || hasLaunched)
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
            Drag();
        }

        // Player has released the cat
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Launch();
        }
    }

    private void Drag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 offset = mousePosition - launchPoint.position;

        // Limit how far the cat can be dragged from the launch point
        offset = Vector3.ClampMagnitude(offset, maxDragDistance);

        Vector3 targetPosition = launchPoint.position + offset;
        targetPosition.z = launchPoint.position.z;

        catRb.position = targetPosition;

        dragLine.SetPosition(0, leftBandAnchor.position);
        dragLine.SetPosition(1, targetPosition);
        dragLine.SetPosition(2, rightBandAnchor.position);
    }

    private void Launch()
    {
        hasLaunched = true;
        Vector3 dragVector = launchPoint.position - catRb.position;

        catRb.isKinematic = false;
        catRb.AddForce(dragVector * slingshotPower, ForceMode.Impulse);

        dragLine.enabled = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            Mathf.Abs(mainCamera.transform.position.z - launchPoint.position.z)
        );

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = launchPoint.position.z;

        return worldPosition;
    }
}
