using UnityEngine;

public class DumpsterController : MonoBehaviour
{
    public delegate void LevelCompleteHandler();
    public event LevelCompleteHandler LevelComplete;

    [SerializeField]
    [Tooltip("The position of the clearance box taht is checked for obstructions above the lid")]
    private Vector3 lidClearancePos = new Vector3(0f, 8.44f, 0f);

    [SerializeField]
    [Tooltip("The size of the clearance box that is checked for obstructions above the lid.")]
    private Vector3 lidClearanceSize = new Vector3(12.2f, 1f, 7.5f);

    private ParticleSystem particles;
    private Animator animator;
    private AudioSource audioSource;
    private bool isOpen = false;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cat")
        {
            particles.Play();
            audioSource.Play();
            LevelComplete?.Invoke();
        }
    }

    private void Update()
    {
        if (IsLidBlocked())
        {
            animator.SetBool("isOpen", false);
        }
        else
        {
            isOpen = true;
            animator.SetBool("isOpen", true);
        }
    }

    private bool IsLidBlocked()
    {
        Vector3 centre = transform.TransformPoint(lidClearancePos);

        Collider[] hits = Physics.OverlapBox(centre, lidClearanceSize / 2, transform.rotation);

        foreach (Collider hit in hits)
        {
            if (!hit.transform.IsChildOf(transform) && !isOpen)
            {
                return true;
            }
        }

        return false;
    }
}
