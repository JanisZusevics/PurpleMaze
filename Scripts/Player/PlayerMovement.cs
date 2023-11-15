using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 50f;
    public bool isActive = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if  (isActive == true)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(movement * speed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerMovement otherPlayer = collision.gameObject.GetComponent<PlayerMovement>();
        if (otherPlayer != null && isActive && !otherPlayer.isActive)
        {
            otherPlayer.isActive = true;
        }
    }
}