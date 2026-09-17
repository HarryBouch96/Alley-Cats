using UnityEngine;
using UnityEngine.InputSystem;

public class CatController : MonoBehaviour
{
    public delegate void ShotFinishedHandler(CatController cat);
    public event ShotFinishedHandler ShotFinished;
    public delegate void ReachedApexHandler();
    public event ReachedApexHandler ReachedApex;
    public delegate void PouncedHandler();
    public event PouncedHandler Pounced;

    [Header("Player turn settings")]
    [Space(10)]
    [SerializeField]
    [Tooltip(
        "Threshold for how slowly the cat has to be moving for it to be considered stationary."
    )]
    private float stoppedSpeed = 0.1f;

    [SerializeField]
    [Tooltip("How many seconds the cat has to be stationary before the turn ends.")]
    private float stoppedDuration = 3f;

    [SerializeField]
    [Tooltip("The audio clip to play when the cat pounces.")]
    private AudioClip pounceAudioClip;

    private Rigidbody rb;
    private Bounds levelBounds;
    private float stoppedTimer;
    private AudioSource audioSource;
    private bool hasLaunched;
    private bool hasBounds;
    private bool hasPounced = false;
    private bool hasReachedApex = false;

    // Cache the Rigidbody before LevelManager starts the first shot
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hasPounced = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (hasLaunched && !hasReachedApex && rb.linearVelocity.y < 0f)
        {
            hasReachedApex = true;
            ReachedApex?.Invoke();
        }

        CheckCatStopped();
        CheckCatOutOfBounds();
    }

    private void Update()
    {
        CheckPounce();
    }

    private void CheckCatOutOfBounds()
    {
        if (hasLaunched && hasBounds)
        {
            Vector3 position = rb.position;

            bool inside =
                position.x >= levelBounds.min.x
                && position.x <= levelBounds.max.x
                && position.y >= levelBounds.min.y
                && position.y <= levelBounds.max.y;

            if (inside)
            {
                return;
            }

            // Set hasLaunched back to false so shot
            // completion is only announced once
            hasLaunched = false;
            ShotFinished?.Invoke(this);
        }
    }

    private void CheckCatStopped()
    {
        if (!hasLaunched)
        {
            return;
        }

        // A timer prevents temporary slowdowns
        // from prematurely ending the shot
        if (rb.linearVelocity.magnitude < stoppedSpeed)
        {
            stoppedTimer += Time.fixedDeltaTime;
        }
        else
        {
            stoppedTimer = 0f;
        }

        if (stoppedTimer >= stoppedDuration)
        {
            // Set hasLaunched back to false so shot
            // completion is only announced once
            hasLaunched = false;
            ShotFinished?.Invoke(this);
        }
    }

    public void ResetForShot(Vector3 position)
    {
        // Temporarily enable physics so the previous
        // shot's velocities can be cleared
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Prevent physics from affecting the cat until it is launched
        rb.isKinematic = true;
        rb.position = position;
        rb.rotation = Quaternion.identity;

        stoppedTimer = 0f;
        hasLaunched = false;
        hasPounced = false;
        hasReachedApex = false;
    }

    public void SetAimPosition(Vector3 position)
    {
        rb.position = position;
    }

    public void Launch(Vector3 impulse)
    {
        hasLaunched = true;
        rb.isKinematic = false;
        rb.AddForce(impulse, ForceMode.Impulse);
    }

    public void CheckPounce()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && hasLaunched && hasPounced == false)
        {
            hasPounced = true;
            audioSource.PlayOneShot(pounceAudioClip);
            Vector3 force = new Vector3(10f, -30f, 0);
            rb.AddForce(force, ForceMode.Impulse);
            Pounced?.Invoke();
        }
    }

    public void SetLevelBounds(Bounds bounds)
    {
        levelBounds = bounds;
        hasBounds = true;
    }
}
