using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public List<GameObject> players;
    public TMP_Text activePlayersText;
    public TMP_Text gameOverText;
    public TMP_Text collectablesText;
    public Vector3 playerAverageLocation;

    private int activePlayers;
    private int collectablesCollected = 0;

    private void Start()
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        activePlayers = players.Count;
        UpdateActivePlayersText();
        gameOverText.gameObject.SetActive(false);
        UpdateCollectablesText();
    }

    private void Update()
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        UpdateActivePlayersCount();
        UpdateActivePlayersText();
        UpdatePlayerAverageLocation();
    }

    public void PlayerDied()
    {
        activePlayers--;
        UpdateActivePlayersText();

        if (activePlayers <= 0)
        {
            gameOverText.gameObject.SetActive(true);
        }
    }

    public void CollectableCollected()
    {
        collectablesCollected++;
        UpdateCollectablesText();
    }

    private void UpdateActivePlayersText()
    {
        activePlayersText.text = "Active Players: " + activePlayers;
    }

    private void UpdateCollectablesText()
    {
        collectablesText.text = "Collectables: " + collectablesCollected;
    }

    private void UpdateActivePlayersCount()
    {
        players.RemoveAll(player => player == null);
        activePlayers = players.Count(player => player.GetComponent<PlayerMovement>().isActive);
    }

    private void UpdatePlayerAverageLocation()
    {
        Vector3 sum = Vector3.zero;
        int activePlayersCount = 0;

        foreach (GameObject player in players)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.isActive)
            {
                activePlayersCount++;
                sum += player.transform.position;
            }
        }

        if (activePlayersCount > 0)
        {
            playerAverageLocation = sum / activePlayersCount;
        }
    }
}