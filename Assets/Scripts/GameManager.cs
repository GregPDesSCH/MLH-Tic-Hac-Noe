/* 
    MLH Tic-Hac-Noe
    Game Manager

    This script manages the entire game through execution.

    Programmed by Gregory Desrosiers 
    Candidate for Bachelor of Software Engineering (University of Waterloo)

    Programming Date: December 3, 2016
    Comments Added and Additional Fixes: December 18, 2016

    File Name: GameManager.cs
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CurrentState { PLAYER_ONE_TOKEN_ONE, PLAYER_ONE_TOKEN_TWO, PLAYER_TWO_TOKEN_ONE, PLAYER_TWO_TOKEN_TWO, POPUP_MESSAGE, GAME_OVER };

public class GameManager : MonoBehaviour
{
    // We use the singleton design principle, where only one instance of this can be used at any time throughout execution.
    public static GameManager gameManager;

    /* Game Stats Properties */
    public CurrentState currentState;
    private bool moveMade; // Trigger variable
    private int spacesRemaining = 0;
    private int numberOfPlayerOneWins = 0; // Player 1 Score
    private int numberOfPlayerTwoWins = 0; // Player 2 Score

    /* Game Objects */
    // We use object pooling for the tokens for performance instead of just instantiating and deleting them accordingly.
    public GameObject[] tokenSpaces;
    private GameObject[] cubeTokens = new GameObject[36]; 
    private GameObject[] cylinderTokens = new GameObject[36];

    // These hold our blueprints to the tokens, as instances are made from them. (Not the same idea as objects built from classes)
    public GameObject cubeTokenPrefab;
    public GameObject cylinderTokenPrefab;

    // UI Elements
    public Text gameStateText;
    public GameObject popupPanel;
    public Text popupText;
    public Button okButton;
    public Button playAgainButton;

    public Text player1ScoreText;
    public Text player2ScoreText;


    // At the start of running the game, the Start function is instantiated. This follows Unity's Execution Order
    // of Event Functions, which you can read along here: https://docs.unity3d.com/Manual/ExecutionOrder.html
    void Start ()
    {
        // Initialize the singleton variable accordingly.
        if (gameManager != null)
            Destroy(gameObject);

        gameManager = this;

        // Set our game management variables.
        moveMade = false;
        spacesRemaining = 36;

        // We build 36 orange cubes and 36 capsules from our blueprints referenced here.
        // By default, they start operating on their own, which goes against having a game. So we just disable them.
        for (int i = 0; i < tokenSpaces.Length; i++)
        {
            cubeTokens[i] = (GameObject)Instantiate(cubeTokenPrefab, tokenSpaces[i].transform.position, Quaternion.identity);
            cylinderTokens[i] = (GameObject)Instantiate(cylinderTokenPrefab, tokenSpaces[i].transform.position, Quaternion.identity);
            cubeTokens[i].SetActive(false);
            cylinderTokens[i].SetActive(false);
        }

        // Adds event listeners to buttons
        okButton.onClick.AddListener(() => RespondToOKButtonOnPopup());
        playAgainButton.onClick.AddListener(() => RestartGame());
        
        // Finally, choose a player and start the game.
        ChoosePlayer();
    }

    void Update()
    {
        // Wait for a mouse click to be made in a space.
        if (moveMade)
        {
            moveMade = false; // Reset trigger
            
            // Check for a game over after a move has been made. If yes, respond accordingly.
            if (spacesRemaining == 0 || Player1Wins() || Player2Wins())
            {
                gameStateText.text = "GAME OVER";
                currentState = CurrentState.GAME_OVER;
                

                if (spacesRemaining == 0) // Is the game board completely full?
                    OpenResultsPopup(0);
                else if (Player1Wins()) // Does Player 1 have a row, column, or diagonal of orange cubes?
                {
                    OpenResultsPopup(1);
                    numberOfPlayerOneWins++;
                }
                else // Does Player 2 have a row, column, or diagonal of blue capsules?
                {
                    OpenResultsPopup(2);
                    numberOfPlayerTwoWins++;
                }

                UpdatePlayerScoreText();
            }

            // Transitions between states of the game
            if (currentState == CurrentState.PLAYER_ONE_TOKEN_ONE)
            {
                currentState = CurrentState.PLAYER_ONE_TOKEN_TWO;
                gameStateText.text = "Player 1 Token 2";
            }
            else if (currentState == CurrentState.PLAYER_ONE_TOKEN_TWO)
            {
                currentState = CurrentState.PLAYER_TWO_TOKEN_ONE;
                gameStateText.text = "Player 2 Token 1";
            }
            else if (currentState == CurrentState.PLAYER_TWO_TOKEN_ONE)
            {
                currentState = CurrentState.PLAYER_TWO_TOKEN_TWO;
                gameStateText.text = "Player 2 Token 2";
            }
            else if (currentState == CurrentState.PLAYER_TWO_TOKEN_TWO)
            {
                currentState = CurrentState.PLAYER_ONE_TOKEN_ONE;
                gameStateText.text = "Player 1 Token 1";
            }
        }
    }

    // Chooses a player out of random.
    void ChoosePlayer()
    {
        // There is roughly a 50% chance that the first player will be Player 1, and another 50% for Player 2.
        // Unity's Random.value follows a uniform distribution, where the probability of every value is the same.
        if (Random.value <= 0.5f)
        {
            currentState = CurrentState.PLAYER_ONE_TOKEN_ONE;
            gameStateText.text = "Player 1 Token 1";
        }
        else
        {
            currentState = CurrentState.PLAYER_TWO_TOKEN_ONE;
            gameStateText.text = "Player 2 Token 1";
        }
    }

    // Disables all active tokens, resets some game variables, stops particle systems, and starts a new game.
    public void RestartGame()
    {
        // If the game is in progress, we reset the scores.
        if (currentState != CurrentState.GAME_OVER && currentState != CurrentState.POPUP_MESSAGE)
        {
            numberOfPlayerOneWins = 0;
            numberOfPlayerTwoWins = 0;
            UpdatePlayerScoreText();
        }

        // We deactivate the tokens and stop any particle systems that are running. Doing so hides the tokens 
        // from being rendered in the camera's frustum.
        for (int i = 0; i < tokenSpaces.Length; i++)
        {
            cubeTokens[i].SetActive(false);
            cylinderTokens[i].SetActive(false);

            cubeTokens[i].GetComponent<ParticleSystem>().Stop();
            cylinderTokens[i].GetComponent<ParticleSystem>().Stop();
        }
        spacesRemaining = 36;
        ChoosePlayer();
    }


    // Updates the players' score on the UI.
    void UpdatePlayerScoreText()
    {
        player1ScoreText.text = "" + numberOfPlayerOneWins;
        player2ScoreText.text = "" + numberOfPlayerTwoWins;
    }

    // Activates a token on the board, if possible. Otherwise, display a popup.
    public void AddTokenToBoard(byte index)
    {
        if (!cubeTokens[index].activeInHierarchy && !cylinderTokens[index].activeInHierarchy &&
            (currentState == CurrentState.PLAYER_ONE_TOKEN_ONE || currentState == CurrentState.PLAYER_ONE_TOKEN_TWO) &&
            !popupPanel.activeInHierarchy)
            cubeTokens[index].SetActive(true);
        else if (!cubeTokens[index].activeInHierarchy && !cylinderTokens[index].activeInHierarchy &&
            (currentState == CurrentState.PLAYER_TWO_TOKEN_ONE || currentState == CurrentState.PLAYER_TWO_TOKEN_TWO) &&
            !popupPanel.activeInHierarchy)
            cylinderTokens[index].SetActive(true);
        else
        {
            popupText.text = "You can\'t place the token there. That space is occupied.";
            popupPanel.SetActive(true);
            return;
        }

        moveMade = true;
        spacesRemaining--;
    }
    
    // Checks if Player 1 has a row, column, or diagonal of six cubes.
    bool Player1Wins()
    {
        // Check all rows. If we have a row of cubes, play the particle systems.
        for (int row = 0; row < 6; row++)
        {
            if (cubeTokens[0 + row * 6].activeInHierarchy && cubeTokens[1 + row * 6].activeInHierarchy && cubeTokens[2 + row * 6].activeInHierarchy
                && cubeTokens[3 + row * 6].activeInHierarchy && cubeTokens[4 + row * 6].activeInHierarchy && cubeTokens[5 + row * 6].activeInHierarchy)
            {
                cubeTokens[0 + row * 6].GetComponent<ParticleSystem>().Play();
                cubeTokens[1 + row * 6].GetComponent<ParticleSystem>().Play();
                cubeTokens[2 + row * 6].GetComponent<ParticleSystem>().Play();
                cubeTokens[3 + row * 6].GetComponent<ParticleSystem>().Play();
                cubeTokens[4 + row * 6].GetComponent<ParticleSystem>().Play();
                cubeTokens[5 + row * 6].GetComponent<ParticleSystem>().Play();
                return true;
            }
                
        }

        // Check all columns. If we have a column of cubes, play the particle systems.
        for (int col = 0; col < 6; col++)
        {
            if (cubeTokens[0 + col].activeInHierarchy && cubeTokens[6 + col].activeInHierarchy && cubeTokens[12 + col].activeInHierarchy
                && cubeTokens[18 + col].activeInHierarchy && cubeTokens[24 + col].activeInHierarchy && cubeTokens[30 + col].activeInHierarchy)
            {
                cubeTokens[0 + col].GetComponent<ParticleSystem>().Play();
                cubeTokens[6 + col].GetComponent<ParticleSystem>().Play();
                cubeTokens[12 + col].GetComponent<ParticleSystem>().Play();
                cubeTokens[18 + col].GetComponent<ParticleSystem>().Play();
                cubeTokens[24 + col].GetComponent<ParticleSystem>().Play();
                cubeTokens[30 + col].GetComponent<ParticleSystem>().Play();
                return true;
            }
            
        }

        // Check the diagonals. If there is a diagonal of cubes, play the particle systems.
        // Backward Slash
        if (cubeTokens[0].activeInHierarchy && cubeTokens[7].activeInHierarchy && cubeTokens[14].activeInHierarchy && cubeTokens[21].activeInHierarchy
            && cubeTokens[28].activeInHierarchy && cubeTokens[35].activeInHierarchy)
        {
            cubeTokens[0].GetComponent<ParticleSystem>().Play();
            cubeTokens[7].GetComponent<ParticleSystem>().Play();
            cubeTokens[14].GetComponent<ParticleSystem>().Play();
            cubeTokens[21].GetComponent<ParticleSystem>().Play();
            cubeTokens[28].GetComponent<ParticleSystem>().Play();
            cubeTokens[35].GetComponent<ParticleSystem>().Play();
            return true;
        }


        if (cubeTokens[5].activeInHierarchy && cubeTokens[10].activeInHierarchy && cubeTokens[15].activeInHierarchy && cubeTokens[20].activeInHierarchy
            && cubeTokens[25].activeInHierarchy && cubeTokens[30].activeInHierarchy)
        {
            cubeTokens[5].GetComponent<ParticleSystem>().Play();
            cubeTokens[10].GetComponent<ParticleSystem>().Play();
            cubeTokens[15].GetComponent<ParticleSystem>().Play();
            cubeTokens[20].GetComponent<ParticleSystem>().Play();
            cubeTokens[25].GetComponent<ParticleSystem>().Play();
            cubeTokens[30].GetComponent<ParticleSystem>().Play();
            return true;
        }

        return false;
    }

    // Checks if Player 2 has a row, column, or diagonal of six cubes.
    bool Player2Wins()
    {
        // Check all rows. If we have a row of capsules, play the particle systems.
        for (int row = 0; row < 6; row++)
        {
            if (cylinderTokens[0 + row * 6].activeInHierarchy && cylinderTokens[1 + row * 6].activeInHierarchy && cylinderTokens[2 + row * 6].activeInHierarchy
                && cylinderTokens[3 + row * 6].activeInHierarchy && cylinderTokens[4 + row * 6].activeInHierarchy && cylinderTokens[5 + row * 6].activeInHierarchy)
            {
                cylinderTokens[0 + row * 6].GetComponent<ParticleSystem>().Play();
                cylinderTokens[1 + row * 6].GetComponent<ParticleSystem>().Play();
                cylinderTokens[2 + row * 6].GetComponent<ParticleSystem>().Play();
                cylinderTokens[3 + row * 6].GetComponent<ParticleSystem>().Play();
                cylinderTokens[4 + row * 6].GetComponent<ParticleSystem>().Play();
                cylinderTokens[5 + row * 6].GetComponent<ParticleSystem>().Play();
                return true;
            }
        }

        // Check all columns. If we have a column of capsules, play the particle systems.
        for (int col = 0; col < 6; col++)
        {
            if (cylinderTokens[0 + col].activeInHierarchy && cylinderTokens[6 + col].activeInHierarchy && cylinderTokens[12 + col].activeInHierarchy
                && cylinderTokens[18 + col].activeInHierarchy && cylinderTokens[24 + col].activeInHierarchy && cylinderTokens[30 + col].activeInHierarchy)
            {
                cylinderTokens[0 + col].GetComponent<ParticleSystem>().Play();
                cylinderTokens[6 + col].GetComponent<ParticleSystem>().Play();
                cylinderTokens[12 + col].GetComponent<ParticleSystem>().Play();
                cylinderTokens[18 + col].GetComponent<ParticleSystem>().Play();
                cylinderTokens[24 + col].GetComponent<ParticleSystem>().Play();
                cylinderTokens[30 + col].GetComponent<ParticleSystem>().Play();
                return true;
            }
        }

        // Check the diagonals. If there is a diagonal of capsules, play the particle systems.
        // Backward Slash
        if (cylinderTokens[0].activeInHierarchy && cylinderTokens[7].activeInHierarchy && cylinderTokens[14].activeInHierarchy &&
            cylinderTokens[21].activeInHierarchy && cylinderTokens[28].activeInHierarchy && cylinderTokens[35].activeInHierarchy)
        {
            cylinderTokens[0].GetComponent<ParticleSystem>().Play();
            cylinderTokens[7].GetComponent<ParticleSystem>().Play();
            cylinderTokens[14].GetComponent<ParticleSystem>().Play();
            cylinderTokens[21].GetComponent<ParticleSystem>().Play();
            cylinderTokens[28].GetComponent<ParticleSystem>().Play();
            cylinderTokens[35].GetComponent<ParticleSystem>().Play();
            return true;
        }

        if (cylinderTokens[5].activeInHierarchy && cylinderTokens[10].activeInHierarchy && cylinderTokens[15].activeInHierarchy &&
            cylinderTokens[20].activeInHierarchy && cylinderTokens[25].activeInHierarchy && cylinderTokens[30].activeInHierarchy)
        {
            cylinderTokens[5].GetComponent<ParticleSystem>().Play();
            cylinderTokens[10].GetComponent<ParticleSystem>().Play();
            cylinderTokens[15].GetComponent<ParticleSystem>().Play();
            cylinderTokens[20].GetComponent<ParticleSystem>().Play();
            cylinderTokens[25].GetComponent<ParticleSystem>().Play();
            cylinderTokens[30].GetComponent<ParticleSystem>().Play();
            return true;
        }

        return false;
    }

    // Activates the popup panel with appropriate message.
    void OpenResultsPopup(byte messageIndex)
    {
        // This ternary operator determines what message to show.
        popupText.text = (messageIndex == 0) ? "We have a draw..." : "Player " + messageIndex + " wins!";
        popupPanel.SetActive(true);
    }
    
    // Deactivates the popup message.
    void RespondToOKButtonOnPopup()
    {
        popupPanel.SetActive(false);
    }
}
