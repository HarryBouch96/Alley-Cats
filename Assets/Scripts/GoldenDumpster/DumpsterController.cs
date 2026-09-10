using Unity.VisualScripting;
using UnityEngine;

public class DumpsterController : MonoBehaviour
{
    public delegate void LevelCompleteHandler();
    public event LevelCompleteHandler LevelComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cat")
        {
            LevelComplete?.Invoke();
        }
    }
}
