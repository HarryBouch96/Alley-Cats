using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialHint : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
        }
    }
}
