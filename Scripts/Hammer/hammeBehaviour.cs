using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Hammer behaviour script using math animations 
/// </summary>
public class hammeBehaviour : MonoBehaviour
{

    public GameObject crown;
    private Vector3 crownPosition;
    private GameObject hammer;
    private Vector3 desiredPosition;
    public float floatingHeight = 30.0f;
    private Vector3 velocity = Vector3.zero; // Reference velocity for the SmoothDamp method
    public float hammerHeightSize = 50f;
    public float elasticSpeed = 0.5f;
    public float elasticSpeedIncrease = 0.1f;


    public enum State
    {
        Awakening,
        Following,
        Striking
    }
    private State currentState;
    void Awake()
    {
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

        switch (currentState)
        {
            case State.Awakening:
                // while hammer is not within 1f of desired position
                if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                {
                    // move hammer towards desired position
                    ElasticMovement(desiredPosition);
                }
                else
                {
                    // set state to following
                    enterState(State.Following);
                }

                break;
            case State.Following:
                // set desired position to crown position
                getDesiredPosition();
                // while hammer is not within 1f of desired position
                if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                {
                    // log elastic speed
                    Debug.Log(elasticSpeed);
                    elasticSpeed = elasticSpeed*0.97f;
                    // move hammer towards desired position
                    ElasticMovement(desiredPosition, elasticSpeed);
                }
                else
                {
                    // set state to striking
                    getDesiredPosition(true);
                    enterState(State.Striking);
                }
                break;
            case State.Striking:
                // set desired position to crown position
                // while hammer is not within 1f of desired position
                if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                {
                    // move hammer towards desired position
                    AggressiveMovement(desiredPosition,0.5f);
                }
                else
                {
                    // set state to following
                    enterState(State.Following);
                }
                break;
        }
    }



    private void getDesiredPosition(bool Strike = false)
    {
        crownPosition = crown.transform.position;
        // set desired position to crown position
        desiredPosition = crownPosition;
        // set desired position y to 0
        if (Strike)
        {
            desiredPosition.y = hammerHeightSize;
        }
        else
        {
            desiredPosition.y = floatingHeight;
        }
    }


    /// <summary>
    /// Moves the hammer smoothly towards the desired position
    /// </summary>
    /// <param name="desiredPosition"></param>
    private void ElasticMovement(Vector3 desiredPosition, float smoothTime = 0.5f)
    {
        // Set the desired position
        this.desiredPosition = desiredPosition;
        // Calculate the distance between the current position and the desired position
        float distance = Vector3.Distance(transform.position, desiredPosition);
        // If the distance is greater than 0.1f
        if (distance > 0.1f)
        {
            // Move the hammer towards the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
    private void AggressiveMovement(Vector3 desiredPosition, float acceleration = 0.1f)
    {
        // Set the desired position
        this.desiredPosition = desiredPosition;
        // Calculate the distance between the current position and the desired position
        float distance = Vector3.Distance(transform.position, desiredPosition);
        // If the distance is greater than 0.1f
        if (distance > 0.1f)
        {
            // Increase speed over time
            acceleration += Time.deltaTime;
            // Move the hammer towards the desired position
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, acceleration);
        }
        else
        {
            // Reset speed when the desired position is reached
            acceleration = 0.1f;
        }
    }
    private void Telegraph()
    {
        //  store curennt desired position height 
        float currentHeight = desiredPosition.y;
        // set new desired position height 
        desiredPosition.y = floatingHeight+10;
        // while hammer is not within 1f of desired position
        if (Vector3.Distance(transform.position, desiredPosition) > 1f)
        {
            // move hammer towards desired position
            AggressiveMovement(desiredPosition, 0.5f);
        }
        else
        {
            // set state to striking
            currentState = State.Striking;
        }
    }

    // execute code when entering state
    private void enterState(State state)
    {
        // log state change
        Debug.Log($"State changed to {state}");
        switch (state)
        {
            case State.Awakening:
                // set desired position to crown position
                getDesiredPosition();
                break;
            case State.Following:
                // set desired position to crown position
                getDesiredPosition();
                elasticSpeed = 0.5f;
                // set statw to following
                currentState = State.Following;
                break;
            case State.Striking:
                // set desired position to crown position
                getDesiredPosition(true);
                // set state to striking
                currentState = State.Striking;
                break;
        }
    }
}
