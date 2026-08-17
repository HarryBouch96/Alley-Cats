using System.Collections;
using UnityEngine;

public class camControl : MonoBehaviour
{
    Camera cam;
    GameObject player;

    // Ray ray;
    public Vector3 offset;
    public Transform camFollowTransform;
    public BoxCollider2D bounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ray = new Ray(
        //     cam.transorm.position,
        //     (player.transform.position - GetComponent<Camera>().transform.position).normalized
        // );
        this.transform.position = new Vector3(
            camFollowTransform.position.x + offset.x,
            this.transform.position.y + offset.y,
            camFollowTransform.position.z + offset.z
        );
    }
}
