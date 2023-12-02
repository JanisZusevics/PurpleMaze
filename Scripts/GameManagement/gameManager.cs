using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI statsCanvas; 
    public TextMeshProUGUI endScreen; 

    private int activePlayers = 0;
    private int collectablesCollected = 0;

    void Start()
    {

    }

    public void PlayerStateChanged(bool isActive)
    {
        if (isActive) activePlayers++;
        else activePlayers--;

        if (activePlayers < 1)
        {
            // log game over
            Debug.Log("Game Over");
            // set end screen as active
            endScreen.gameObject.SetActive(true);
            // log end screen enabled  
            Debug.Log("End Screen Enabled");
            endScreen.text = $"Game Over\nCollectables: {collectablesCollected}";
        }

        statsCanvas.text = $"Mice: {activePlayers}\nOrbs: {collectablesCollected}";
    }

    public void IncrementCollectablesCollected()
    {
        collectablesCollected++;
    }
}