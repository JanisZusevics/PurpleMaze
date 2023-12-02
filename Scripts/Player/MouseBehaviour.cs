using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    public float speedFactor = 1f; // Speed factor for the mouse movement
    public float range = 1f;       // Range within which the mouse does not move towards the playerMover
    private GameManager gameManager; // GameManager instance

    public bool _isActive = false;
    private bool _isDead = false;   // Tracks if the mouse is dead
    private bool _isOnGround = true; // Tracks if the mouse is on the ground
    private int groundContactCount = 0; // Tracks the number of ground contacts

    public bool isKing = false;
    public enum MouseState
    {
        Asleep,
        Idle,
        Moving,
        Ragdoll,
        Death
    }

    public MouseState currentState;

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

        UpdateState(); // Initialize the state
    }

    void Update()
    {
        switch (currentState)
        {
            case MouseState.Moving:
                // debug dra a blue line towards the playerMover
                
                Debug.DrawLine(transform.position, gameManager.King.transform.position, Color.blue);
                break;
            case MouseState.Ragdoll:
                // Ragdoll logic here
                break;
            case MouseState.Idle:
                // Idle logic here
                //check if the mouse is on the ground and the playerMover is outside range
                if (_isOnGround && gameManager.King != null)
                {
                    //if the mouse is on the ground and the playerMover is outside range, set the state to moving
                    currentState = MouseState.Moving;
                }
                break;
            case MouseState.Death:
                IsActive = false;
                //gameManager.PlayerStateChanged(_isActive);
                isKing = false;
                gameManager.theKingIsDead();
                Destroy(gameObject);
                break;
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
        else if (_isActive && gameManager.King != null && Vector3.Distance(transform.position, gameManager.King.transform.position) > range)
        {
            currentState = MouseState.Moving;
        }
        else
        {
            currentState = MouseState.Idle;
        }
    }
    public void kingMe(){
        isKing = true;
    }
}
