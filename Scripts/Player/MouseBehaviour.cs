using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{

    private GameManager gameManager; // GameManager instance

    public bool _isActive = false;
    private bool _isDead = false;   // Tracks if the mouse is dead
    private bool _isOnGround = true; // Tracks if the mouse is on the ground
    private int groundContactCount = 0; // Tracks the number of ground contacts

    public bool isKing = false;
    
    private GameObject MouseCrown;
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
        // find crown child
        MouseCrown = transform.Find("MouseCrown").gameObject;
    }

    void Start()
    {

        UpdateState(); // Initialize the state
    }

    void Update()
    {
        if (isKing)
        {
            // Toggle crown
            MouseCrown.SetActive(true);
        }
        else
        {
            // remove the crown
            MouseCrown.SetActive(false);
        }
        switch (currentState)
        {
            case MouseState.Moving:
                // Moving logic here
                // log moving 
                Debug.DrawLine(transform.position, gameManager.Crown.transform.position, Color.blue);
                break;
            case MouseState.Ragdoll:
                // Ragdoll logic here
                break;
            case MouseState.Idle:
                // Idle logic here
                // log idle 
                Debug.Log("Idle");
                //check if the mouse is on the ground 
                if (_isOnGround)
                {
                    //if the mouse is on the ground and the playerMover is outside range, set the state to moving
                    currentState = MouseState.Moving;
                }
                break;
            case MouseState.Death:
                IsActive = false;
                //gameManager.PlayerStateChanged(_isActive);
                if (isKing)
                {
                    isKing = false;
                    gameManager.theKingIsDead();
                }
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
        // if collide with a corwn object 
        else if (collision.gameObject.CompareTag("Crown"))
        {
            // if the mouse is not the king
            if (!isKing)
            {
                IsActive = true;
                // set the mouse to be the king
                isKing = true;
                // set the king in the game manager
                gameManager.appointKing(gameObject);
                // log the king
                Debug.Log("King");
                // set the collided crown to be inactive
                collision.gameObject.SetActive(false);
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
        else if (_isActive )
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
