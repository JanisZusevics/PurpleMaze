using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monarchy : MonoBehaviour
{
    public GameObject King; // The target GameObject
    public bool appointedKing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void appointKing(GameObject dent){
        King = dent;

        appointedKing = true;
    }
    public void theKingIsDead(){
        appointedKing = false;
        King = null;
    }

}
