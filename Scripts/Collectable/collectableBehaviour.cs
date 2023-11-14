using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Called when the collectable is collected
    public void Collect()
    {
        // Increment the collected count and print it to the log
        CollectableSpawner.instance.CollectableCollected();
        
        // Deactivate the collectable and move it below the ground
        gameObject.SetActive(false);
        transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
    }

    // OnTriggerEnter is called when the Collider other enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Print the tag of the colliding object
        Debug.Log("Collided with object: " + other.gameObject.name + " with tag: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
        else
        {
            // Optionally, handle the collision with objects that are not the player
        }
    }
}
