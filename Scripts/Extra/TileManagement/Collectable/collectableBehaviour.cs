using UnityEngine;

public class Collectable : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }
    // Called when the collectable is collected
    public void Collect()
    {
        // Increment the collected count and print it to the log
        gameManager.CollectableCollected();
        
        // Destroy the collectable 
        Destroy(gameObject);
    }

    // OnTriggerEnter is called when the Collider other enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerMovement>().isActive)
        {
            Collect();
        }
        else
        {
            // Optionally, handle the collision with objects that are not the player
        }
    }
}