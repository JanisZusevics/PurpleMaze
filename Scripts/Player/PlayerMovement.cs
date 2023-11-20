using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 velocity { get; private set; } // Expose the velocity of the player

    private Rigidbody rb;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = rb.position;
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

        // Calculate velocity based on the distance moved since the last frame
        velocity = (rb.position - lastPosition) / Time.deltaTime;

        lastPosition = rb.position;
    }
}