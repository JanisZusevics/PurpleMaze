using UnityEngine;
using System.Collections;

public class HammerManager : MonoBehaviour
{
    public GameObject hammerPrefab;
    public GameObject playerMover;  // Empty object that player objects move towards
    public float timeMin = 0.0f;
    public float timeMax = 2.0f;
    public bool isHammerEnabled = true; // Add this line

    private void Start()
    {
        Debug.Log("Starting HammerManager...");
        if (isHammerEnabled)
        {
            StartCoroutine(StartAfterDelay());
        }
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        if (isHammerEnabled)
        {
            SpawnHammerStrike();
        }
    }

    private void SpawnHammerStrike()
    {
        Vector3 averagePosition = playerMover.transform.position;

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

    private void HandleStrikeEnd()
    {
        if (isHammerEnabled)
        {
            StartCoroutine(WaitAndSpawn());
        }
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(Random.Range(timeMin, timeMax));
        if (isHammerEnabled)
        {
            SpawnHammerStrike();
        }
    }
}