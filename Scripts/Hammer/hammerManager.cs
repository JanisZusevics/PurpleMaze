using UnityEngine;
using System.Collections;
using System.Linq;

public class HammerManager : MonoBehaviour
{
    public GameObject hammerPrefab;
    public GameObject[] players;
    private Vector3 averagePosition;

    public float timeMin = 0.0f;
    public float timeMax = 2.0f;

    private void Start()
    {
        Debug.Log("Starting HammerManager...");
        StartCoroutine(StartAfterDelay());
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        SpawnHammerStrike();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player")
            .Where(player => player.GetComponent<PlayerMovement>().isActive)
            .ToArray();   
            averagePosition = GetAveragePosition();
            
         }
            

    private void SpawnHammerStrike()
    {
        Debug.Log("Attempting to spawn a hammer strike...");
        
        if (averagePosition != Vector3.zero)
        {
            averagePosition.y = 0; // Set y-coordinate to 0
            GameObject hammer = Instantiate(hammerPrefab, averagePosition, Quaternion.identity);
            HammerStrike hammerStrike = hammer.GetComponent<HammerStrike>();
            if (hammerStrike != null)
            {
                hammerStrike.OnStrikeEnd += HandleStrikeEnd;
                hammerStrike.InitializeStrike(averagePosition);
            }
            else
            {
                Debug.Log("HammerStrike component not found on the instantiated hammer.");
            }
        }
        else
        {
            Debug.Log("Average position is zero, not spawning a hammer.");
        }
    }

    private void HandleStrikeEnd()
    {
        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(Random.Range(timeMin, timeMax));
        SpawnHammerStrike();
    }

    private Vector3 GetAveragePosition()
    {
        if (players.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;
        foreach (GameObject player in players)
        {
            sum += player.transform.position;
        }

        return sum / players.Length;
    }
}