using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text activePlayersText;
    public TMP_Text gameOverText;

    private int activePlayers;

    private void Start()
    {
        activePlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        UpdateActivePlayersText();
        gameOverText.gameObject.SetActive(false);
    }

   private void Update()
    {
        UpdateActivePlayersCount();
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

    private void UpdateActivePlayersText()
    {
        activePlayersText.text = "Active Players: " + activePlayers;
    }

private void UpdateActivePlayersCount()
{
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    activePlayers = 0;

    foreach (GameObject player in players)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.isActive)
        {
            activePlayers++;
        }
    }

    activePlayersText.text = "Active Players: " + activePlayers;

    if (activePlayers <= 0)
    {
        gameOverText.gameObject.SetActive(true);
    }
}
}