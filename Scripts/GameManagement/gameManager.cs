using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI statsCanvas; 
    public TextMeshProUGUI endScreen; 

    private int activePlayers = 0;
    private int collectablesCollected = 0;

    public GameObject crown;
    public GameObject King = null; // The target GameObject
    public GameObject dentPrefab;
    public bool appointedKing = false;

    void Start()
    {
        // spawn in a dent prefab 
        King = Instantiate(dentPrefab, transform.position, transform.rotation);
        King.GetComponent<MouseBehaviour>().isKing = true;
        King.GetComponent<MouseBehaviour>().IsActive = true;
        appointKing(King);
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

    public void appointKing(GameObject dent){
        King = dent;
        dent.GetComponent<MouseBehaviour>().isKing = true;
        appointedKing = true;
    }
    public void theKingIsDead(){
        appointedKing = false;
        King = null;
    }
}