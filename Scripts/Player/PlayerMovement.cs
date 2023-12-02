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
            // if mouse behvaviour state is moving 
        if (MouseBehaviour.currentState == MouseBehaviour.MouseState.Moving)
        {
            if (MouseBehaviour.isKing == true)
            {
                KingMover();
            }
            else
            {
                MoveTowardsKing();
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
        float distance = Vector3.Distance(transform.position, gameManager.King.transform.position);
        if (distance > range)
        {
            // Rotate to face the playerMover object 
            // with 90 degrees y offset
            Vector3 direction = gameManager.King.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
            float distance_multiplier = Mathf.Clamp(distance / 4.5f, 1, 100f);
            Debug.Log($"Distance Multiplier: {distance_multiplier}");
            // Move forward, with speed directly proportional to the distance from the playerMover
            transform.Translate(Vector3.forward * speedFactor * distance_multiplier * Time.deltaTime);
        }
    }
}
    

