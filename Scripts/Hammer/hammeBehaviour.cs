using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammeBehaviour : MonoBehaviour
{

    public GameObject crown;
    private GameObject hammer;

    # list states 
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

        crown = GameObject.Find("Crown");
        currentState = State.Awakening;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        crownPosition = crown.transform.position;

        ElasticMovement(desiredPosition);
        switch (currentState)
        {
            case State.Awakening:
                break;
            case State.Following:
                break;
            case State.Striking:
                break;
        }
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
