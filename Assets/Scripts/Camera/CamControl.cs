using System.Collections;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField]
    [Tooltip("The time in seconds for the camera to catch up to the desired position.")]
    private float smoothTime = 0.3f;

    [Header("Deadzone")]
    [SerializeField]
    [Tooltip(
        "The width and height of the deadzone as percentages of the camera's width and height."
    )]
    private Vector2 deadzoneSize = new Vector2(0.75f, 0.5f);

    [Header("Bounds")]
    [SerializeField]
    [Tooltip(
        "The Environment's 2D box collider that defines the width and height of the map bounds."
    )]
    private BoxCollider2D mapBounds;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private Transform target;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Get the target's viewport coordinates
        // X and Y run from (0,0) at bottom-left to (1,1) at top-right
        // Z is its depth in front of the camera, in world units
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

        float deadzoneLeft = (1f - deadzoneSize.x) / 2f;
        float deadzoneRight = 1f - deadzoneLeft;
        float deadzoneBottom = (1f - deadzoneSize.y) / 2f;
        float deadzoneTop = 1f - deadzoneBottom;

        Vector3 desiredPos = transform.position;

        // Find the nearest point inside the deadzone
        Vector3 deadzonePoint = new Vector3(
            Mathf.Clamp(viewportPos.x, deadzoneLeft, deadzoneRight),
            Mathf.Clamp(viewportPos.y, deadzoneBottom, deadzoneTop),
            viewportPos.z
        );

        // Convert that point to world coordinates at the target's depth
        Vector3 deadzoneWorldPoint = cam.ViewportToWorldPoint(deadzonePoint);

        // Move just far enough to bring the target back to the deadzone edge
        if (viewportPos.x < deadzoneLeft || viewportPos.x > deadzoneRight)
        {
            desiredPos.x += target.position.x - deadzoneWorldPoint.x;
        }

        if (viewportPos.y < deadzoneBottom || viewportPos.y > deadzoneTop)
        {
            desiredPos.y += target.position.y - deadzoneWorldPoint.y;
        }

        // Clamp the desired position to the map bounds
        // accounting for the camera's width and height
        if (mapBounds != null)
        {
            // Calculate width and height of the camera view at the target depth
            float depth = target.position.z - cam.transform.position.z;
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));

            float halfViewWidth = (topRight.x - bottomLeft.x) / 2f;
            float halfViewHeight = (topRight.y - bottomLeft.y) / 2f;

            Bounds bounds = mapBounds.bounds;

            desiredPos.x = Mathf.Clamp(
                desiredPos.x,
                bounds.min.x + halfViewWidth,
                bounds.max.x - halfViewWidth
            );

            desiredPos.y = Mathf.Clamp(
                desiredPos.y,
                bounds.min.y + halfViewHeight,
                bounds.max.y - halfViewHeight
            );
        }

        // Move the camera smoothly to the desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smoothTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
