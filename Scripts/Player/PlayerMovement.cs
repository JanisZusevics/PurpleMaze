using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Joystick joystick;

    public float pushForce = 40f;
    public float speed = 10f;
    public float minimumFollowSpeed = 1f;

    public float maximumFollowSpeed = 100f;
    public Vector3 velocity { get; private set; }

    private Rigidbody rb;
    private Vector3 lastPosition;
    private GameManager gameManager;
    private MouseBehaviour MouseBehaviour;

    // Boid behavior variables
    public float viewDistance = 5f;
    public float separationThreshold = 1f;
    public float alignmentWeight = 0.1f;
    public float cohesionWeight = 0.1f;
    public float separationWeight = 5f;
    public float crownWeight = 0.4f;
    public float joystickWeight = 0.5f;

    // Turning behavior variables
    public float maxTurnSpeed = 5f;
    public float turnAcceleration = 1f;
    public float velocityAcceleration = 1f;

    private float currentTurnSpeed = 0f;
    private float currentVelocity = 0f;

    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        MouseBehaviour = GetComponent<MouseBehaviour>();
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
            if (MouseBehaviour.isKing)
            {
                KingMover();
            }
            else
            {
                Vector3 boidDirection = CalculateBoidBehavior();
                Vector3 crownDirection = (gameManager.Crown.transform.position - transform.position).normalized;
        
                Vector3 combinedDirection = boidDirection * (1 - crownWeight) + crownDirection * crownWeight;
                Move(combinedDirection);
            }
            // gradually move x and z rotation to 0
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0.1f);

        }
    }

    void KingMover()
    {
        joystick.gameObject.SetActive(true);
        float moveHorizontal = joystick.Horizontal;
        float moveVertical = joystick.Vertical;
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(rb.position + movement * (speed * 2) * Time.deltaTime);

        velocity = (rb.position - lastPosition) / Time.deltaTime;
        lastPosition = rb.position;



        // push all mice away from the king in a radius 
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance/2);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Mouse") && hitCollider.gameObject.GetComponent<MouseBehaviour>().IsActive && hitCollider.gameObject != gameObject)
            {
                // debug draw into sky from mouse
                Debug.DrawLine(hitCollider.transform.position, hitCollider.transform.position + Vector3.up * 100, Color.red);
                // get the rigidbody of the mouse
                Rigidbody otherRb = hitCollider.GetComponent<Rigidbody>();
                // get the direction from the mouse to the king
                Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
                // rotate mouse to face away from king
                hitCollider.transform.rotation = Quaternion.Slerp(hitCollider.transform.rotation, Quaternion.LookRotation(-direction), 0.05F);
                // apply force to mouse
                otherRb.AddForce(-direction * pushForce);
            }
        }
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
        DrawDebugRays();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Mouse") && hitCollider.gameObject != gameObject)
            {
                Rigidbody otherRb = hitCollider.GetComponent<Rigidbody>();

                alignVelocity += otherRb.velocity;
                alignCount++;

                cohesionPoint += hitCollider.transform.position;
                cohesionCount++;

                Vector3 diff = transform.position - hitCollider.transform.position;
                if (diff.magnitude < separationThreshold)
                {
                    separationDirection += diff.normalized;
                    separationCount++;
                }
            }
        }

        if (alignCount > 0) alignVelocity /= alignCount;
        if (cohesionCount > 0) cohesionPoint = (cohesionPoint / cohesionCount) - transform.position;
        if (separationCount > 0) separationDirection /= separationCount;

        float currentTurnSpeed = CalculateTurnSpeed(separationDirection, separationCount);

        // get joystick direction
        Vector3 joystickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        Vector3 boidDirection = (alignVelocity * alignmentWeight 
                               + cohesionPoint * cohesionWeight 
                               + separationDirection * separationWeight * currentTurnSpeed 
                               + joystickDirection * joystickWeight).normalized;
        return boidDirection;
    }

    float CalculateTurnSpeed(Vector3 separationDirection, int separationCount)
    {
        float currentTurnSpeed = maxTurnSpeed;
        if (separationCount > 0)
        {
            float closestMouseDistance = separationDirection.magnitude;
            currentTurnSpeed += turnAcceleration / closestMouseDistance;
        }
        // log the current and max speed
        Debug.Log($"Current Turn Speed: {currentTurnSpeed}\nMax Turn Speed: {maxTurnSpeed}");
        return Mathf.Min(currentTurnSpeed, maxTurnSpeed);
    }

    void Move(Vector3 direction)
    {
        // Orient the mouse to face the direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed * Time.deltaTime);

        velocity = transform.forward * speed;
        // shoot a ray out directly in front of the mouse at the distance of viewDistance
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, viewDistance))
        {
            // if the ray hits something
            if (hit.collider.CompareTag("Mouse"))
            {
                // log the hit
                Debug.Log("Hit Mouse");
                if (hit.collider.gameObject.GetComponent<MouseBehaviour>().isKing == false)
                {
                    // get the rigidbody of the hit object
                    Rigidbody otherRb = hit.collider.GetComponent<Rigidbody>();
                    // rotate the otherRb to face away from the me
                    //otherRb.transform.rotation = Quaternion.Slerp(otherRb.transform.rotation, Quaternion.LookRotation(-direction), 0.05F);
                    // apply force to move the otherRB forward
                    //otherRb.AddForce(otherRb.transform.forward * speed);
                }
                //  gradually rotate to the left 
                transform.Rotate(Vector3.up * Time.deltaTime * 10);
                // gradually lower the speed to minimumSpeed
                velocity = Vector3.Lerp(velocity, Vector3.zero, 0.5f);
                rb.MovePosition(rb.position + velocity * Time.deltaTime);
                return;
            }
        }
        // Calculate the distance to the crown
        float distance = (gameManager.Crown.transform.position - transform.position).magnitude;

        // Calculate the direction to the crown
        Vector3 directionToCrown = (gameManager.Crown.transform.position - transform.position).normalized;

        // Calculate the desired turn speed and velocity based on the distance to the crown
        float desiredTurnSpeed = Mathf.Min(turnAcceleration * distance, maxTurnSpeed);
        float desiredVelocity = Mathf.Min(velocityAcceleration * distance, maximumFollowSpeed);

        // Gradually increase the current turn speed and velocity towards the desired values
        float turnSpeed = Mathf.Lerp(currentTurnSpeed, desiredTurnSpeed, Time.deltaTime * turnAcceleration);
        float calculatedVelocity = Mathf.Lerp(currentVelocity, desiredVelocity, Time.deltaTime * velocityAcceleration);

        // Update the current turn speed and velocity
        currentTurnSpeed = turnSpeed;
        currentVelocity = calculatedVelocity;

        // Rotate towards the crown
        float singleStep = turnSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, directionToCrown, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        // Move towards the crown
        rb.velocity = transform.forward * calculatedVelocity;
    }

    void DrawDebugRays()
    {
        Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.green);
        Debug.DrawRay(transform.position, transform.right * viewDistance, Color.green);
        Debug.DrawRay(transform.position, transform.up * viewDistance, Color.green);
        Debug.DrawRay(transform.position, -transform.forward * viewDistance, Color.green);
        Debug.DrawRay(transform.position, -transform.right * viewDistance, Color.green);
        Debug.DrawRay(transform.position, -transform.up * viewDistance, Color.green);
    }

}
