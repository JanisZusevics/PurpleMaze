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

    public float speedFactor = 1f; // Speed factor for the mouse movement
    public float range = 1f;       // Range within which the mouse does not move towards the playerMover
    public float separationDistance = 2f; // Distance within which the mouse will separate from other mice
    public float circleSpeed = 3f; // Speed at which the mouse circles around the playerMover

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
                Vector3 separationForce = CalculateSeparationForce();
                ApplyForce(separationForce);
                float distance = Vector3.Distance(transform.position, gameManager.Crown.transform.position);
                if (distance > range)
                {
                    // log moving to king 
                    Debug.Log("Moving to King");
                    MoveTowardsKing();
                }
                else
                {
                    // log circling king
                    Debug.Log("Circling King");
                    CircleAroundKing();
                }
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
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        // Calculate velocity based on the distance moved since the last frame
        velocity = (rb.position - lastPosition) / Time.deltaTime;
        lastPosition = rb.position;
        // Rotate to face the direction of movement
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
    }
    // Move towards king
    void MoveTowardsKing()
    {

        // Calculate the distance to the playerMover object
        float distance = Vector3.Distance(transform.position, gameManager.Crown.transform.position);
        // if king exists we chase the king 
        if (gameManager.kingExists == true)
        {
            // log chasing king
            // Calculate a speed multiplier based on the distance
            // Use a logarithmic function to make the speed drop off more aggressively near the range limit
            float speedMultiplier = Mathf.Log(Mathf.Max(0, distance - range) + 1);

            // Rotate to face the crown
            Vector3 direction = gameManager.Crown.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            // Move towards the crown, with speed directly proportional to the distance
            transform.Translate(Vector3.forward * speed * speedMultiplier * Time.deltaTime);
        }
        // if king does not exist we chase the crown 
        else
        {
            // log chasing crown
            // Move towards the crown object
            Vector3 direction = gameManager.Crown.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
            // calculate speed
            Vector3 desiredSpeed = Vector3.forward * speedFactor * 10 * Time.deltaTime;

            // set the desired speed to at least the minimum speed
            desiredSpeed = Vector3.ClampMagnitude(desiredSpeed, speedFactor);
            // log speed
            //Debug.Log($"Speed: {desiredSpeed}");
            transform.Translate(desiredSpeed);
        }
    }
    Vector3 CalculateSeparationForce()
    {
        Vector3 separationForce = Vector3.zero;
        int nearbyMiceCount = 0;

        foreach (var otherMouse in gameManager.AllMice) // Assuming AllMice is a list of all mice
        {
            if (otherMouse != this && Vector3.Distance(transform.position, otherMouse.transform.position) < separationDistance)
            {
                Vector3 difference = transform.position - otherMouse.transform.position;
                if (difference.magnitude > Mathf.Epsilon)
                {
                    separationForce += difference.normalized / difference.magnitude;
                    nearbyMiceCount++;
                }
            }
        }

        if (nearbyMiceCount > 0)
        {
            separationForce /= nearbyMiceCount;
        }

        return separationForce;
    }


    void CircleAroundKing()
    {
        Vector3 toKing = gameManager.King.transform.position - transform.position;
        if (toKing.magnitude > Mathf.Epsilon)
        {
            Vector3 tangentialForce = Vector3.Cross(Vector3.up, toKing).normalized;

            float distance = toKing.magnitude;
            if (distance > range)
            {
                // Move towards the king
                Vector3 seekForce = toKing.normalized * speed;
                ApplyForce(seekForce);
            }

            // Add the tangential force to create circling behavior
            ApplyForce(tangentialForce * circleSpeed);
        }
    }


    void ApplyForce(Vector3 force)
    {
        if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z))
        {
            Debug.LogError("NaN force detected: " + force);
            return;
        }
        rb.AddForce(force);
    }


}
    

