using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // LINKS
    private GameManager gameManager; // GameManager instance
    public Joystick joystick; // Joystick instance


    // GENERAL variables    
    public Vector3 velocity { get; private set; } // Current velocity
    private Rigidbody rb;
    public float viewDistance = 5f;


    // SPEED variables
    public float speed = 10f; // Movement speed
    public float velocityAcceleration = 1f; // Acceleration of velocity
    private float currentVelocity = 0f; // Current velocity
    public float maximumFollowSpeed = 10f; // Maximum follow speed


    // Boid behavior variables
    public float separationThreshold = 1f; // Distance threshold for separation 
    // Boid behavior weights
    public float alignmentWeight = 0.1f;
    public float cohesionWeight = 0.1f;
    public float separationWeight = 5f;
    public float crownWeight = 0.4f;
    public float joystickWeight = 0.5f;


    // Turning behavior variables
    public float maxTurnSpeed = 5f;
    public float turnAcceleration = 1f;
    private float currentTurnSpeed = 0f;


    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        joystick = GameObject.FindObjectOfType<Joystick>();
        rb = GetComponent<Rigidbody>();
    }

    public void Follow()
    {
        Vector3 boidDirection = CalculateBoidBehavior(); // Calculate the direction the mouse should move in
        //Debug.Log($"Boid Direction: {boidDirection}");// log boid direction
        Vector3 crownDirection = (gameManager.Crown.transform.position - transform.position).normalized; // Calculate the direction to the crown
        Vector3 combinedDirection = boidDirection * (1 - crownWeight) + crownDirection * crownWeight; // Combine the boid direction and crown direction
        Debug.Log($"Combined Direction: {combinedDirection}"); // log combined direction
        Move(combinedDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0.1f); // Orient the mouse to face the direction of movement on the x and z axes

    }

    // Calculate the direction the mouse should move in 
    Vector3 CalculateBoidBehavior()
    {
        // Initialize variables 
        Vector3 alignVelocity = Vector3.zero;
        Vector3 cohesionPoint = Vector3.zero;
        Vector3 separationDirection = Vector3.zero;

        int alignCount = 0;
        int cohesionCount = 0;
        int separationCount = 0;

        // Get all mice within the view distance
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);
        DrawDebugRays();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Mouse") && hitCollider.gameObject != gameObject)
            {
                Rigidbody otherRb = hitCollider.GetComponent<Rigidbody>();

                alignVelocity += otherRb.velocity; // Add the velocity of the mouse to the alignment velocity
                alignCount++; // Increment the alignment count

                cohesionPoint += hitCollider.transform.position; // Add the position of the mouse to the cohesion point
                cohesionCount++; // Increment the cohesion count

                Vector3 diff = transform.position - hitCollider.transform.position; // Calculate the difference between the mouse and the other mouse
                float distance = diff.magnitude; // Calculate the distance between the mouse and the other mouse 
                if (distance < separationThreshold) // If the distance between the mouse and the other mouse is less than the separation threshold
                {
                    Vector3 separationForce = diff.normalized / distance; // Calculate the separation force
                    // Add the direction from the other mouse to the mouse to the separation direction
                    separationDirection += separationForce; 
                    separationCount++;
                }
            }
        }
        // Calculate the average alignment velocity, cohesion point, and separation direction
        if (alignCount > 0) 
        {
            alignVelocity = (alignVelocity / alignCount).normalized;
        }
        if (cohesionCount > 0) 
        {
            cohesionPoint = ((cohesionPoint / cohesionCount) - transform.position).normalized;
        }
        if (separationCount > 0) 
        {
            separationDirection = (separationDirection / separationCount).normalized;

        }

        float currentTurnSpeed = CalculateTurnSpeed(separationDirection, separationCount);

        // get joystick direction
        Vector3 joystickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        
        Vector3 boidDirection = (alignVelocity * alignmentWeight 
                               + cohesionPoint * cohesionWeight 
                               + -separationDirection * separationWeight * currentTurnSpeed 
                               + joystickDirection * joystickWeight).normalized;
        // log boid direction with weights
        Debug.Log($"Boid Direction: {boidDirection}\nAlign Velocity: {alignVelocity}\nCohesion Point: {cohesionPoint}\nSeparation Direction: {separationDirection}\nJoystick Direction: {joystickDirection}");
        return boidDirection;
    }
    // Calculate the turn speed based on the separation direction and number of mice within the separation threshold
    float CalculateTurnSpeed(Vector3 separationDirection, int separationCount)
    {
        float currentTurnSpeed = maxTurnSpeed;
        // if there are any mice within the separation threshold
        if (separationCount > 0)
        {
            // calculate the distance to the closest mouse
            float closestMouseDistance = separationDirection.magnitude;
            // gradually increase the turn speed based on the distance to the closest mouse
            // the closer the mouse, the faster the turn speed
            currentTurnSpeed += turnAcceleration / closestMouseDistance;
            // magnify the turn speed of the mouse by the number of mice within the separation threshold
            currentTurnSpeed *= separationCount;
        }
        // log the current and max speed
        //Debug.Log($"Current Turn Speed: {currentTurnSpeed}\nMax Turn Speed: {maxTurnSpeed}");
        return Mathf.Min(currentTurnSpeed, maxTurnSpeed);
    }

    // Move the mouse in the specified direction
    void Move(Vector3 direction)
    {
        // Orient the mouse to face the direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed * Time.deltaTime);

        velocity = transform.forward * speed;
  

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
