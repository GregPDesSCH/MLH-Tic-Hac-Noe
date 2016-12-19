/* 
    MLH Tic-Hac-Noe
    Token Space Listener

    This script is the listener to the clicks on the spaces of the game board.

    Programmed by Gregory Desrosiers 
    Candidate for Bachelor of Software Engineering (University of Waterloo)

    Programming Date: December 3, 2016
    Comments Added and Additional Fixes: December 18, 2016

    File Name: GameManager.cs
*/

using UnityEngine;
using System.Collections;

public class TokenSpaceListener : MonoBehaviour
{
    public byte arrayIndex = 0; // Index of the board space

    // Responds to mouse clicks on the colliders.
    void OnMouseUp()
    {
        // While the game is in progress, see if the token can be added on mouse click.
        if (GameManager.gameManager.currentState != CurrentState.GAME_OVER &&
            GameManager.gameManager.currentState != CurrentState.POPUP_MESSAGE)
            GameManager.gameManager.AddTokenToBoard(arrayIndex);
    }
}
