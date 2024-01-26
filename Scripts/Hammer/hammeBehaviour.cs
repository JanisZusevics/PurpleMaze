using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammeBehaviour : MonoBehaviour
{

    public GameObject crown;
    private Vector3 crownPosition;
    private GameObject hammer;
    private Vector3 desiredPosition;

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
        hammer.transform.position = new Vector3(crown.transform.position.x, crown.transform.position.y + 20, crown.transform.position.z);

        currentState = State.Awakening;
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


        ElasticMovement(desiredPosition);
        switch (currentState)
        {
            case State.Awakening:
                // while hammer is not within 1f of desired position
                if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                {
                    // move hammer towards desired position
                    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5);
                }
                else
                {
                    // set state to following
                    currentState = State.Following;
                }

                break;
            case State.Following:
                // set desired position to crown position
                getDesiredPosition();
                // while hammer is not within 1f of desired position
                if (Vector3.Distance(transform.position, desiredPosition) > 1f)
                {
                    // move hammer towards desired position
                    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5);
                }
                break;
            case State.Striking:
                break;
        }
    }



    private void getDesiredPosition()
    {
        crownPosition = crown.transform.position;
        // set desired position to crown position
        desiredPosition = crownPosition;
        // set desired position y to 0
        desiredPosition.y = 30;
    }

    /// <summary>
    /// Moves the hammer smoothly towards the desired position
    /// </summary>
    /// <param name="desiredPosition"></param>
    private void ElasticMovement(Vector3 desiredPosition)
    {
        // Set the desired position
        this.desiredPosition = desiredPosition;
        // Calculate the distance between the current position and the desired position
        float distance = Vector3.Distance(transform.position, desiredPosition);
        // If the distance is greater than 0.1f
        if (distance > 0.1f)
        {
            // Move the hammer towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5);
        }
    }  
}
