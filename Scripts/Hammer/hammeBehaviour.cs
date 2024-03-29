using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Hammer behaviour script using math animations 
/// </summary>
public class hammeBehaviour : MonoBehaviour
{

    private GameManager gameManager;
    public GameObject crown;
    private Vector3 crownPosition;
    private GameObject hammer;
    private Vector3 desiredPosition;
    public float floatingHeight = 30.0f;
    private Vector3 velocity = Vector3.zero; // Reference velocity for the SmoothDamp method
    public float hammerHeightSize = 8f;
    public float elasticSpeed = 0.5f;
    public float elasticSpeedIncrease = 1.2f;
    public Vector3 desiredRotation = new Vector3(0, 0, 0);

    public float launchRadius = 30.0f;
    public float launchMultiplier = 50000.0f;



    public enum State
    {
        Awakening,
        Following,
        Telegraphing,
        Striking
    }
    private State currentState;
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        crown = gameManager.Crown;
        // Set hammer as self 
        hammer = this.gameObject;
        // set hammer position high above the crown
        hammer.transform.position = new Vector3(crown.transform.position.x, crown.transform.position.y + (floatingHeight*3), crown.transform.position.z);
        // set state to awakening  
        enterState(State.Awakening);
    }
    // Start is called before the first frame update
    void Start()
    {
        crown = GameObject.Find("Crown");

        crownPosition = crown.transform.position;
        getDesiredPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // if king exists OR in awakening state
        if (gameManager.kingExists || currentState == State.Awakening)
        {
            // if king is in ragdoll state
            if (gameManager.King.GetComponent<MouseBehaviour>().currentState == MouseBehaviour.MouseState.Ragdoll)
            {
                // set state to awakening
                enterState(State.Awakening);
            }
            switch (currentState)
            {
                case State.Awakening:
                    // while hammer is not within 1f of desired position
                    if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                    {
                        // move hammer towards desired position
                        ElasticMovement();
                    }
                    else
                    {
                        // set state to following
                        enterState(State.Following);
                    }

                    break;
                case State.Following:
                    // set desired position to crown position
                    if (gameManager.King.GetComponent<MouseBehaviour>().currentState != MouseBehaviour.MouseState.Ragdoll)
                    { getDesiredPosition(yOffSet: floatingHeight); }

                    // while hammer is not within 1f of desired position
                    Rotator(desiredRotation.x, desiredRotation.y, desiredRotation.z);
                    if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                    {
                        elasticSpeed = elasticSpeed * 0.97f;
                        // move hammer towards desired position
                        ElasticMovement(elasticSpeed);
                    }
                    else
                    {
                        enterState(State.Telegraphing);
                    }
                    break;
                case State.Telegraphing:
                    // set desired position to crown position
                    // while hammer is not within 1f of desired position
                    Rotator(desiredRotation.x, desiredRotation.y, desiredRotation.z);
                    if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                    {
                        // move hammer towards desired position
                        AggressiveMovement(acceleration: 0.7f);
                    }
                    else
                    {
                        // set state to striking
                        enterState(State.Striking);
                    }
                    break;
                case State.Striking:
                    // set desired position to crown position
                    // while hammer is not within 1f of desired position
                    Rotator(desiredRotation.x, desiredRotation.y, desiredRotation.z);
                    if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                    {
                        // move hammer towards desired position
                        AggressiveMovement(acceleration: 0.75f);
                    }
                    else
                    {
                        Strike();
                        enterState(State.Following);
                    }
                    break;
            }
        }
        else if (currentState != State.Awakening)
        {
            // set desired position to crown position
            desiredPosition.y = 100;
            // while hammer is not within 1f of desired position
            ElasticMovement();
        }
    }



    private void getDesiredPosition(float xOffSet = 0, float yOffSet = 0, float zOffSet = 0)
    {
        crownPosition = crown.transform.position;
        // set desired position to crown position
        desiredPosition = crownPosition;

        // Add offsets to desired position
        desiredPosition.x += xOffSet;
        desiredPosition.y += yOffSet;
        desiredPosition.z += zOffSet;  

        // calculate the y angle required to face the desired position
        float yAngle = Mathf.Atan2(desiredPosition.x - transform.position.x, desiredPosition.z - transform.position.z) * Mathf.Rad2Deg;
        // add a 90 degree offset to the angle
        yAngle += 90;

        // set the desired rotation to the calculated angles
        desiredRotation = new Vector3(0, yAngle, desiredRotation.z);  
    }


    /// <summary>
    /// Moves the hammer smoothly towards the desired position
    /// </summary>
    /// <param name="desiredPosition"></param>
    private void ElasticMovement(float smoothTime = 0.5f)
    {
        // Calculate the distance between the current position and the desired position
        float distance = Vector3.Distance(transform.position, desiredPosition);
        // If the distance is greater than 0.1f
        if (distance > 0.1f)
        {
            // Move the hammer towards the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
    private void AggressiveMovement(float acceleration = 0.1f)
    {
        // Calculate the distance between the current position and the desired position
        float distance = Vector3.Distance(transform.position, desiredPosition);
        // If the distance is greater than 0.1f
        if (distance > 0.1f)
        {
            // Increase speed over time
            acceleration += Time.deltaTime * elasticSpeedIncrease;
            // Move the hammer towards the desired position
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, acceleration);
        }
        else
        {
            // Reset speed when the desired position is reached
            acceleration = 0.1f;
        }
    }
    private void Rotator(float xRotation, float yRotation, float zRotation)
    {
        // Set the desired rotation
        Quaternion desiredRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        // Calculate the distance between the current rotation and the desired rotation
        float distance = Quaternion.Angle(transform.rotation, desiredRotation);        // If the distance is greater than 0.1f
        if (distance > 0.1f)
        {
            // Rotate the hammer towards the desired rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, 1f);
        }
    }

    private void Strike()
    {
        // Launch collider 
        // Create a second, larger collider
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, launchRadius);
        // Apply an upward force to player objects
        foreach (Collider hitCollider in hitColliders)
            {
                MouseBehaviour mouseBehaviour = hitCollider.gameObject.GetComponent<MouseBehaviour>();
                if (mouseBehaviour != null && mouseBehaviour.IsActive)
                {
                    // log mouse found 
                    Debug.Log("Mouse Found");
                    Rigidbody playerRigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                    if (playerRigidbody != null)
                    {
                        // Calculate the force based on the distance to the player
                        float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                        float force = Mathf.Log(launchRadius / distance) * launchMultiplier; // Adjust the multiplier as needed

                        // Calculate the direction from the hammer to the mouse
                        Vector3 direction = (hitCollider.transform.position - transform.position).normalized + Vector3.up;
                        
                        // log difection times force
                        Debug.Log(direction * force);
                        // Apply the force in the calculated direction
                        playerRigidbody.AddForce(direction * force, ForceMode.Impulse);
                }
            }
        }
    }

    // execute code when entering state
    private void enterState(State state)
    {
        switch (state)
        {
            case State.Awakening:
                desiredRotation = new Vector3(desiredRotation.x, desiredRotation.y, 0);
                // set desired position to crown position
                getDesiredPosition(yOffSet: floatingHeight);
                // set state to awakening
                currentState = State.Awakening;
                break;
            case State.Following:
                desiredRotation = new Vector3(desiredRotation.x, desiredRotation.y, 0);
                // set desired position to crown position
                elasticSpeed = 0.5f;
                // set statw to following
                currentState = State.Following;
                break;

            case State.Telegraphing:
                desiredRotation = new Vector3(desiredRotation.x, desiredRotation.y, -55);
                // set desired position to crown position
                desiredPosition.y += hammerHeightSize*2;
                // set state to telegraphing
                currentState = State.Telegraphing;
                break;
            case State.Striking:
                desiredRotation = new Vector3(desiredRotation.x, desiredRotation.y, 0);
                // set desired position to crown position
                desiredPosition.y = hammerHeightSize;
                // set state to striking
                currentState = State.Striking;
                break;
        }
    }
}
