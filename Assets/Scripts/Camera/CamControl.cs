using System.Collections;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    [Header("Follow")]
    public float smoothTime = 0.3f;

    [Header("Deadzones")]
    public Vector2 deadzones = new Vector2(2f, 1f);

    [Header("Bounds")]
    public BoxCollider2D mapBounds;

    private Camera cam;
    private Vector3 startPos;
    private Vector3 velocity = Vector3.zero;
    private Transform target;
    private float camHeight;
    private float camWidth;

    private void Start()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
        startPos = transform.position;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        deadzones.x = (float)(Mathf.Abs(startPos.x - (camWidth * 0.75f)));
        deadzones.y = (float)(Mathf.Abs(startPos.y - (camHeight * 0.5f)));
        Vector3 targetPos = target.position;
        Vector3 currentPos = transform.position;

        Vector3 cutoffPos = targetPos - currentPos;
        Vector3 desiredPos = currentPos;

        if (Mathf.Abs(cutoffPos.x) > deadzones.x / 2f)
        {
            desiredPos.x = targetPos.x - (deadzones.x / 2f * Mathf.Sign(cutoffPos.x));
        }
        if (Mathf.Abs(cutoffPos.y) > deadzones.y / 2f)
        {
            desiredPos.y = targetPos.y - (deadzones.y / 2f * Mathf.Sign(cutoffPos.y));
        }

        if (mapBounds != null)
        {
            Bounds bounds = mapBounds.bounds;
            desiredPos.x = Mathf.Clamp(
                desiredPos.x,
                bounds.min.x + camWidth,
                bounds.max.x - camWidth
            );
            desiredPos.y = Mathf.Clamp(
                desiredPos.y,
                bounds.min.y + camHeight,
                bounds.max.y - camHeight
            );
        }

        desiredPos.z = transform.position.z;

        //make movement smooth
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smoothTime
        );
    }

    private void DrawDeadzone()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadzones.x, deadzones.y, 0));
    }
}
