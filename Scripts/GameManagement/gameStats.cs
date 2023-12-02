using UnityEngine;

public class GameStats : MonoBehaviour
{
    public int activePlayers = 0;
    public int collectablesCollected = 0;

    public void PlayerStateChanged(bool isActive)
    {
        if (isActive) activePlayers++;
        else activePlayers--;
    }

    public void IncrementCollectablesCollected()
    {
        collectablesCollected++;
    }
}