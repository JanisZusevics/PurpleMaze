using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    // LINKS TO OTHER SCRIPTS
    public Joystick joystick;
    private GameManager gameManager;
    private MouseBehaviour MouseBehaviour;
    private Follower follower;

    // KING VARIABLES
    public float pushForce = 40f;

    // GENERAL variables
    public float speed = 10f; // Movement speed
    public Vector3 velocity { get; private set; } // Current velocity
    private Rigidbody rb;
    private Vector3 lastPosition;

    public float viewDistance = 5f;


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


    public void KingMover()
    {
        joystick.gameObject.SetActive(true);
        float moveHorizontal = joystick.Horizontal;
        float moveVertical = joystick.Vertical;
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        // move the king
        rb.MovePosition(rb.position + movement * (speed * 2) * Time.deltaTime);
        // calculate velocity of king for mouse push force by dividing the distance moved by the time it took to move that distance
        velocity = (rb.position - lastPosition) / Time.deltaTime;
        lastPosition = rb.position;



        // push all mice away from the king in a radius 
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Mouse") && hitCollider.gameObject.GetComponent<MouseBehaviour>().IsActive && hitCollider.gameObject != gameObject)
            {
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
        // literally fake shto 
        if (movement != Vector3.zero && MouseBehaviour.currentState == MouseBehaviour.MouseState.Moving)
        {
            // rotate the king to face the direction of movement
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }

    }
}
