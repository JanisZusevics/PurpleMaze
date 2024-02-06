using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{

    private GameManager gameManager; // GameManager instance
    private PlayerMovement playerMovement; // PlayerMovement instance
    private Follower follower; // Follower instance

    public bool _isActive = false;
    private bool _isDead = false;   // Tracks if the mouse is dead
    public bool _isOnGround = true; // Tracks if the mouse is on the ground
    private int groundContactCount = 0; // Tracks the number of ground contacts


    public bool isKing = false;

    public float pushForce = 40f;
    
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
        playerMovement = GetComponent<PlayerMovement>();
        follower = GetComponent<Follower>();
        MouseCrown = transform.Find("MouseCrown").gameObject;
        _isActive = true;

    }

    void Start()
    {
        _isActive = true;
        gameManager.PlayerStateChanged(_isActive);
   
        UpdateState(); // Initialize the state
    }

    /// <summary>
    /// This method is called every frame and is responsible for updating the behavior of the mouse.
    /// </summary>
    void Update() //! This needs refactoring
    {
        if ( transform.position.y < -10)
        {
            IsDead = true;
            UpdateState();
        }
        if (transform.position.y > 2)
        {
            _isOnGround = false;
            UpdateState();
        }
        if (_isOnGround && transform.position.y < 3)
        {
            resetRotation();
            resetRotation();
            resetRotation();
            resetRotation();
            resetRotation();
        }
        if (isKing)
        {
            // Toggle crown
            MouseCrown.SetActive(true);
            // push all other isActive mice away from the king in a radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerMovement.viewDistance*2);
            int count = 0;
            foreach (var hitCollider in hitColliders)
            {
                // log ocunt 
                //Debug.Log($"Count: {count}");
                // if the hit collider is a mouse and is not in layer 8 or 9
                if (hitCollider.CompareTag("Mouse") && hitCollider.gameObject.layer != 8 && hitCollider.gameObject.layer != 9)
                {
                    
                    MouseBehaviour mouseBehaviour = hitCollider.gameObject.GetComponent<MouseBehaviour>();
                    if (mouseBehaviour != null && mouseBehaviour.IsActive && hitCollider.gameObject != gameObject)
                    {
                        count++;
                        
                        // debug draw into sky from mouse
                        Debug.DrawLine(hitCollider.transform.position, hitCollider.transform.position + Vector3.up * 100, Color.red);
                       
                        // get the rigidbody of the mouse
                        Rigidbody otherRb = hitCollider.GetComponent<Rigidbody>();

                        // get the direction from the mouse to the king
                        Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
                        // gradually rotate the mouse away from the king
                        otherRb.rotation = Quaternion.Slerp(otherRb.rotation, Quaternion.LookRotation(-direction), 1f);
                        // add force to the mouse
                        // Add a distance multiplier 
                        float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                        float force = Mathf.Log(playerMovement.viewDistance / distance) * pushForce; // Adjust the multiplier as needed
                        // make force a positive value
                        force = Mathf.Abs(force);

                        // push the mouse away from the king
                        otherRb.AddForce(-direction * force);

                    }
                }
            }
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
                if (isKing)
                {
                    playerMovement.KingMover();
                }
                else
                {
                    follower.Follow();
                }
                // log moving 
                Debug.DrawLine(transform.position, gameManager.Crown.transform.position, Color.blue);
                break;
            case MouseState.Ragdoll:
                if (transform.position.y > 3)
                {
                    // constamtly retate the mouse against the x axis 
                    transform.Rotate(-Vector3.right * 200 * Time.deltaTime);

                }
                else
                {
                    // reset the rotation of the mouse
                    resetRotation();
                }
                if (_isOnGround)
                {
                    resetRotation();
                    UpdateState();
                }
                break;
            case MouseState.Idle:
                // Idle logic here
                //check if the mouse is on the ground 

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


    private void resetRotation()
    {
        // get mouse rotation
        Vector3 rotation = transform.rotation.eulerAngles;
        // set x and z rotation to 0
        rotation.x = 0;
        rotation.z = 0;
        // set the rotation to the new rotation
        transform.rotation = Quaternion.Euler(rotation);
    }
    void OnCollisionEnter(Collision collision)
    {
        // if collide with ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            resetRotation();

            groundContactCount++;
            UpdateState();
        }
        else if (collision.gameObject.CompareTag("Hammer"))
        {
            // log hit
            Debug.Log("Hit");
            IsDead = true;
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
                kingMe();   

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
        gameObject.layer = LayerMask.NameToLayer("King");

    }
}
