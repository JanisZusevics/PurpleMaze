using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    public GameObject playerMover; // The target GameObject
    public float speedFactor = 1f; // Speed factor for the mouse movement
    public float range = 1f;       // Range within which the mouse does not move towards the playerMover
    private GameManager gameManager; // GameManager instance

    private bool _isActive = false;
    private bool _isDead = false;   // Tracks if the mouse is dead
    private bool _isOnGround = true; // Tracks if the mouse is on the ground
    private int groundContactCount = 0; // Tracks the number of ground contacts


    private enum MouseState
    {
        Asleep,
        Idle,
        Moving,
        Ragdoll,
        Death
    }

    private MouseState currentState;

    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                gameManager.PlayerStateChanged(_isActive);
                UpdateState();
            }
        }
    }

    public bool IsDead
    {
        get { return _isDead; }
        set
        {
            if (_isDead != value)
            {
                _isDead = value;
                UpdateState();
            }
        }
    }

    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    void Start()
    {
        if (gameManager != null)
        {
            playerMover = gameManager.playerMover;
        }
        UpdateState(); // Initialize the state
    }

    void Update()
    {
        switch (currentState)
        {
            case MouseState.Moving:
                MoveTowardsPlayerMover();
                // debug dra a blue line towards the playerMover
                Debug.DrawLine(transform.position, playerMover.transform.position, Color.blue);
                break;
            case MouseState.Ragdoll:
                // Ragdoll logic here (if any)
                //check if the mouse is on the ground
                if (_isOnGround)
                {
                    //if the mouse is on the ground, set the state to moving
                    //currentState = MouseState.Moving;
                }
                // while in ragdoll debug draw red line towards the sky
                Debug.DrawLine(transform.position, transform.position + Vector3.up * 10, Color.red);
                break;
            case MouseState.Idle:
                // Idle logic here
                //check if the mouse is on the ground and the playerMover is outside range
                if (_isOnGround && playerMover != null && Vector3.Distance(transform.position, playerMover.transform.position) > range)
                {
                    //if the mouse is on the ground and the playerMover is outside range, set the state to moving
                    currentState = MouseState.Moving;
                }
                break;
            case MouseState.Death:
                IsActive = false;
                //gameManager.PlayerStateChanged(_isActive);
                Destroy(gameObject);
                break;
        }
    }

    private void MoveTowardsPlayerMover()
    {
        if (playerMover != null)
        {
            // Calculate the distance to the playerMover object
            float distance = Vector3.Distance(transform.position, playerMover.transform.position);
            if (distance > range)
            {
                // Rotate to face the playerMover object 
                // with 90 degrees y offset
                Vector3 direction = playerMover.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(direction);

                float distance_multiplier = Mathf.Clamp(distance / 4.5f, 1, 100f);
                Debug.Log($"Distance Multiplier: {distance_multiplier}");
                // Move forward, with speed directly proportional to the distance from the playerMover
                transform.Translate(Vector3.forward * speedFactor * distance_multiplier * Time.deltaTime);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContactCount++;
            UpdateState();
        }
        else if (collision.gameObject.CompareTag("Mouse"))
        {
            MouseBehaviour otherMouse = collision.gameObject.GetComponent<MouseBehaviour>();
            if (otherMouse != null && otherMouse.IsActive && !this.IsActive)
            {
                this.IsActive = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && groundContactCount > 0)
        {
            groundContactCount--;
            UpdateState();
        }
    }  

    private void UpdateState()
    {
        bool isOnGround = groundContactCount > 0;

        if (_isDead)
        {
            currentState = MouseState.Death;
            // log death
            Debug.Log("Mouse Died");
            // Destroy the mouse object

        }
        else if (!_isActive)
        {
            currentState = MouseState.Asleep;
        }
        else if (!isOnGround)
        {
            currentState = MouseState.Ragdoll;
        }
        else if (_isActive && playerMover != null && Vector3.Distance(transform.position, playerMover.transform.position) > range)
        {
            currentState = MouseState.Moving;
        }
        else
        {
            currentState = MouseState.Idle;
        }
    }
}
