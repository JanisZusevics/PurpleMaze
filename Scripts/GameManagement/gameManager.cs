using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI statsCanvas; 
    public TextMeshProUGUI endScreen; 

    private int activePlayers = 0;
    private int collectablesCollected = 0;

    public GameObject Crown;
    public GameObject King = null; // The target GameObject
    public GameObject dentPrefab;
    public bool kingExists = false;

    public GameObject[] AllMice;

    void Awake()
    {
        // spawn in a dent prefab 
        King = Instantiate(dentPrefab, transform.position, transform.rotation);
       // appointKing(King);
        //King.GetComponent<MouseBehaviour>().isKing = true;
        //King.GetComponent<MouseBehaviour>().IsActive = true;

        // spawn in a crown aove the dent prefab at y + 20
        Crown.transform.position = new Vector3(King.transform.position.x, King.transform.position.y + 20, King.transform.position.z);
    }


    void Update()
    {
        // if king exists set the crown to be inside the king
        if (kingExists)
        {
            Crown.transform.position = King.transform.position;
        }

        AllMice = GameObject.FindGameObjectsWithTag("Mouse");
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
        kingExists = true;
        // set the crown object as inactive
        Crown.SetActive(false);
        
    }
    public void theKingIsDead(){
        // set the crown object to be at the position of the dead king
        Crown.transform.position = King.transform.position;
        // set the crown object to be active
        Crown.SetActive(true);
        kingExists = false;
        King = null;
    }
    public void AddMouse(GameObject mouse)
    {
        AllMice = AllMice.Concat(new[] { mouse }).ToArray();
    }
    
}