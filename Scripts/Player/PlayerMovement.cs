using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 velocity { get; private set; } // Expose the velocity of the player

    private Rigidbody rb;
    private Vector3 lastPosition;

    // Reference to the Joystick script
    public Joystick joystick; // Assuming the joystick script is named 'Joystick'

    private GameManager gameManager; // GameManager instance
    private MouseBehaviour MouseBehaviour;



    // Boid behavior variables
    public float viewDistance = 5f;
    public float separationThreshold = 1f;
    public float alignmentWeight = 0.1f;
    public float cohesionWeight = 0.1f;
    public float separationWeight = 5f;
    public float crownWeight = 0.4f; // Importance of moving towards the crown


    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        // assign its own mousebehaviour
        MouseBehaviour = GetComponent<MouseBehaviour>();
        // assign joystick
        joystick = GameObject.FindObjectOfType<Joystick>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = rb.position;
    }

    void Update()
    {
        if (MouseBehaviour.currentState == MouseBehaviour.MouseState.Moving)
        {
            if (MouseBehaviour.isKing == true)
            {
                KingMover();
            }
            else
            {
                    Vector3 boidDirection = CalculateBoidBehavior();
                    Vector3 crownDirection = (gameManager.Crown.transform.position - transform.position).normalized;
        
                    // Combine boid behavior with movement towards the crown
                    Vector3 combinedDirection = boidDirection * (1 - crownWeight) + crownDirection * crownWeight;
                    Move(combinedDirection);
            }
        }
    }

    // Move king with joystick
    void KingMover()
    {
        joystick.gameObject.SetActive(true);
        // Use joystick.Horizontal and joystick.Vertical for movement
        float moveHorizontal = joystick.Horizontal;
        float moveVertical = joystick.Vertical;
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(rb.position + movement * (speed*2) * Time.deltaTime);
        // Calculate velocity based on the distance moved since the last frame
        velocity = (rb.position - lastPosition) / Time.deltaTime;
        lastPosition = rb.position;
        // Rotate to face the direction of movement
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
    }

    Vector3 CalculateBoidBehavior()
        {
            Vector3 alignVelocity = Vector3.zero;
            Vector3 cohesionPoint = Vector3.zero;
            Vector3 separationDirection = Vector3.zero;

            int alignCount = 0;
            int cohesionCount = 0;
            int separationCount = 0;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);
            Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.green);
            Debug.DrawRay(transform.position, transform.right * viewDistance, Color.green);
            Debug.DrawRay(transform.position, transform.up * viewDistance, Color.green);
            Debug.DrawRay(transform.position, -transform.forward * viewDistance, Color.green);
            Debug.DrawRay(transform.position, -transform.right * viewDistance, Color.green);
            Debug.DrawRay(transform.position, -transform.up * viewDistance, Color.green);

            foreach (var hitCollider in hitColliders) {
                if (hitCollider.CompareTag("Mouse") && hitCollider.gameObject != gameObject) {
                    Rigidbody otherRb = hitCollider.GetComponent<Rigidbody>();

                    // Alignment
                    alignVelocity += otherRb.velocity;
                    alignCount++;

                    // Cohesion
                    cohesionPoint += hitCollider.transform.position;
                    cohesionCount++;

                    // Separation
                    Vector3 diff = transform.position - hitCollider.transform.position;
                    if (diff.magnitude < separationThreshold) {
                        separationDirection += diff.normalized;
                        separationCount++;
                    }
                }
            }

            if (alignCount > 0) alignVelocity /= alignCount;
            if (cohesionCount > 0) cohesionPoint = (cohesionPoint / cohesionCount) - transform.position;
            if (separationCount > 0) {
                separationDirection /= separationCount;
                separationDirection = -separationDirection.normalized; // Ensure it's pointing away from neighbors
            }

            // Weighted sum of boid behaviors
            Vector3 boidDirection = (alignVelocity * alignmentWeight 
                                   + cohesionPoint * cohesionWeight 
                                   + separationDirection * separationWeight).normalized;
            return boidDirection;
        }

    void Move(Vector3 direction)
    {
        velocity = direction * speed;
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
        }
    }
}
    

