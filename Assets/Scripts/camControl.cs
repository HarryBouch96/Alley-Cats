using System.Collections;
using UnityEngine;

public class camControl : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float smoothTime = 0.3f;

    [Header("Deadzones")]
    public Vector2 deadzones = new Vector2(2f, 1f);

    private Camera cam;
    private float camHeight;
    private float camWidth;

    private Vector3 velocity = Vector3.zero;

    [Header("Bounds")]
    public BoxCollider2D mapBounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
        Debug.Log(camHeight);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        deadzones.y = (float)(camHeight - (camHeight * 0.1));
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

    void DrawDeadzone()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadzones.x, deadzones.y, 0));
    }
}
