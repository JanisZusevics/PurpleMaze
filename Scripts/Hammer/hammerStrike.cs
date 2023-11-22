using UnityEngine;
using System.Collections;

public class HammerStrike : MonoBehaviour
{
    public float strikeDuration = 1.0f; // Duration for the hammer to strike
    public Material shadowMaterial;
    public Material hammerMaterial;
    public float hammerDiameter;
    public float telegraphDuration;

    private Collider strikeCollider;
    public Renderer hammerRenderer;
    private float timer;
    private Vector3 strikePosition;
    public delegate void StrikeEndHandler();
    public event StrikeEndHandler OnStrikeEnd;

    public ParticleSystem smokeEffectPrefab;
    private ParticleSystem smokeEffect;
    public float smokeDelay = 2.0f; // Reference to the instantiated smoke effect
    public float launchMultiplier = 10.0f; // Multiplier for the launch force

    private enum State
    {
        Initializing,
        Telegraphing,
        Striking,
        Retreating,
        CleaningUp
    }

    private State currentState;

    void Start()
    {
        strikeCollider = GetComponent<Collider>();
        if (hammerRenderer == null)
        {
            Debug.LogWarning("HammerRenderer is not assigned in the editor.");
        }
        strikeCollider.enabled = false;
        currentState = State.Initializing;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Initializing:
                InitializeStrike(strikePosition);
                break;
            case State.Telegraphing:
                TelegraphStrike();
                break;
            case State.Striking:
                PerformStrike();
                break;
            case State.Retreating:
                Retreat();
                break;
            case State.CleaningUp:
                Cleanup();
                break;
        }
    }

    public void InitializeStrike(Vector3 strikePosition)
    {
        // Position the hammer at the strike position and start the telegraph
        this.strikePosition = strikePosition;
        hammerRenderer.material = shadowMaterial; // Change material to shadow material first

        // Scale down the hammer to its initial telegraph size
        transform.localScale = new Vector3(transform.localScale.x, 0.1f, transform.localScale.z); // Only scale y

        hammerRenderer.enabled = true; // Then enable the renderer
        timer = telegraphDuration;
        currentState = State.Telegraphing;
    }

    private void TelegraphStrike()
    {
        // Decrease the timer and scale the hammer based on the timer
        timer -= Time.deltaTime;
        float scale = Mathf.Clamp01((telegraphDuration - timer) / telegraphDuration) * hammerDiameter;
        transform.localScale = new Vector3(scale, 0.01f, scale); // Only scale x and z

        if (timer <= 0)
        {
            currentState = State.Striking;
            strikeCollider.enabled = true;
        }
    }

    private void PerformStrike()
    {
        // Change the y scale of the hammer
        transform.localScale = new Vector3(transform.localScale.x, 2, transform.localScale.z);

        // Enable the strike collider to delete players
        strikeCollider.enabled = true;

        // Create a second, larger collider
        float launchRadius = hammerDiameter * 4;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, launchRadius);

        // Instantiate the smoke effect
        smokeEffect = Instantiate(smokeEffectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        smokeEffect.transform.localScale = new Vector3(hammerDiameter, hammerDiameter, hammerDiameter); // Scale the smoke effect to match the launch radius

        // Apply an upward force to player objects
        foreach (Collider hitCollider in hitColliders)
        {
            MouseBehaviour mouseBehaviour = hitCollider.gameObject.GetComponent<MouseBehaviour>();
            if (mouseBehaviour != null && mouseBehaviour.IsActive)
            {
                Rigidbody playerRigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    // Calculate the force based on the distance to the player
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    float force = Mathf.Log(launchRadius / distance) * launchMultiplier; // Adjust the multiplier as needed

                    // Calculate the direction from the hammer to the mouse
                    Vector3 direction = (hitCollider.transform.position - transform.position).normalized + Vector3.up;

                    // Apply the force in the calculated direction
                    playerRigidbody.AddForce(direction * force, ForceMode.Impulse);
                }
            }
        }
        hammerRenderer.material = hammerMaterial; // Change material back to hammer material
        StartCoroutine(EnableColliderDuringStrike());
        currentState = State.Retreating;
    }

    private void Retreat()
    {
        // Wait for the strike duration, then start the cleanup
        StartCoroutine(WaitForStrikeDuration());
    }

    private void Cleanup()
    {
        OnStrikeEnd?.Invoke();
        // Disable the renderer and destroy the GameObject
        hammerRenderer.enabled = false;
        Destroy(gameObject);
        StartCoroutine(DestroySmokeEffectAfterDelay(smokeDelay));

    }

    IEnumerator EnableColliderDuringStrike()
    {
        // Enable the collider, wait for the strike duration, then disable the collider
        strikeCollider.enabled = true;
        yield return new WaitForSeconds(strikeDuration);
        strikeCollider.enabled = false;
    }

    IEnumerator WaitForStrikeDuration()
    {
        // Wait for the strike duration, then start the cleanup
        yield return new WaitForSeconds(strikeDuration);
        currentState = State.CleaningUp;
    }
    IEnumerator DestroySmokeEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(smokeEffect.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hammer strike collided with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Mouse") && other.gameObject.GetComponent<MouseBehaviour>().IsActive)
        {
            Debug.Log("Hammer strike collided with active player, deleting player...");
            // set mouse isDead to true
            other.gameObject.GetComponent<MouseBehaviour>().IsDead = true;
        }
    }
}