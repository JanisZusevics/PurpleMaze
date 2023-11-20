using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    public GameObject playerMover; // The target GameObject
    private bool _isActive = false;  // Determines if the mouse should move towards the target
    public float speedFactor = 1f; // Speed factor for the mouse movement
    public float range = 1f;       // Range within which the mouse does not move towards the playerMover
    private GameManager gameManager; // GameManager instance

    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                gameManager.PlayerStateChanged(_isActive);
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
    }

    void Update()
    {
        if (IsActive && playerMover != null)
        {
            float distance = Vector3.Distance(transform.position, playerMover.transform.position);
            if (distance > range)
            {
                MoveTowardsPlayerMover();
            }
        }
    }

    private void MoveTowardsPlayerMover()
    {
        // Rotate to face the playerMover object
        Vector3 direction = playerMover.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        // Move forward (assuming forward is the facing direction of the mouse)
        transform.Translate(Vector3.forward * speedFactor * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Mouse"))
        {
            MouseBehaviour otherMouse = collision.gameObject.GetComponent<MouseBehaviour>();
            if (otherMouse != null && otherMouse.IsActive == true && this.IsActive == false)
            {
                this.IsActive = true;
            }
        }
    }
}