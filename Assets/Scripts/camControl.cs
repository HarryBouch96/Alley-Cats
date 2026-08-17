using UnityEngine;

public class camControl : MonoBehaviour
{
    Camera cam;
    GameObject player;
    Ray ray;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        ray = new Ray(
            cam.transorm.position,
            (player.transform.position - camera.transform.position).normalized
        );
    }
}
