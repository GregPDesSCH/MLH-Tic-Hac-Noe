using UnityEngine;
using System.Collections;

public class TokenSpaceListener : MonoBehaviour
{
    public byte arrayIndex = 0;

    // Mouse click listener
    void OnMouseUp()
    {
        if (GameManager.gameManager.currentState != CurrentState.GAME_OVER && !GameManager.gameManager.AddTokenToBoard(arrayIndex))
            Debug.Log("Space already used.");
    }
}
