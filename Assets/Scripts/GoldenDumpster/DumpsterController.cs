using UnityEngine;

public class DumpsterController : MonoBehaviour
{
    public delegate void LevelCompleteHandler();
    public event LevelCompleteHandler LevelComplete;

    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cat")
        {
            particles.Play();
            LevelComplete?.Invoke();
        }
    }
}
