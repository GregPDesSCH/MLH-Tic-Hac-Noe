using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CurrentState { PLAYER_ONE_TOKEN_ONE, PLAYER_ONE_TOKEN_TWO, PLAYER_TWO_TOKEN_ONE, PLAYER_TWO_TOKEN_TWO, POPUP_MESSAGE, GAME_OVER };

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;


    // Game Stats Properties
    public CurrentState currentState;
    private bool moveMade;
    private int spacesRemaining = 0;
    private int numberOfPlayerOneWins = 0;
    private int numberOfPlayerTwoWins = 0;

    // Game Objects
    public GameObject[] tokenSpaces;
    private GameObject[] cubeTokens = new GameObject[36];
    private GameObject[] cylinderTokens = new GameObject[36];

    public GameObject cubeTokenPrefab;
    public GameObject cylinderTokenPrefab;

    public Text gameStateText;
    public GameObject popupPanel;
    public Text popupText;
    public Button okButton;
    public Button playAgainButton;

    public Text player1ScoreText;
    public Text player2ScoreText;
    

	// Use this for initialization
	void Start ()
    {
        if (gameManager != null)
            Destroy(gameManager);

        gameManager = this;

        moveMade = false;
        spacesRemaining = 36;

        for (int i = 0; i < tokenSpaces.Length; i++)
        {
            cubeTokens[i] = (GameObject)Instantiate(cubeTokenPrefab, tokenSpaces[i].transform.position, Quaternion.identity);
            cylinderTokens[i] = (GameObject)Instantiate(cylinderTokenPrefab, tokenSpaces[i].transform.position, Quaternion.identity);
            cubeTokens[i].SetActive(false);
            cylinderTokens[i].SetActive(false);
        }

        okButton.onClick.AddListener(() => RespondToOKButtonOnPopup());
        playAgainButton.onClick.AddListener(() => RestartGame());

        ChoosePlayer();
    }

    void Update()
    {
        if (moveMade)
        {
            moveMade = false;

            if (spacesRemaining == 0 || Player1Wins() || Player2Wins())
            {
                gameStateText.text = "GAME OVER";
                currentState = CurrentState.GAME_OVER;
                popupPanel.SetActive(true);

                if (spacesRemaining == 0)
                    OpenResultsPopup(0);
                else if (Player1Wins())
                {
                    OpenResultsPopup(1);
                    numberOfPlayerOneWins++;
                }
                else
                {
                    OpenResultsPopup(2);
                    numberOfPlayerTwoWins++;
                }

                UpdatePlayerScoreText();
            }

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

    void ChoosePlayer()
    {
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

    public void RestartGame()
    {
        if (currentState != CurrentState.GAME_OVER)
        {
            numberOfPlayerOneWins = 0;
            numberOfPlayerTwoWins = 0;
            UpdatePlayerScoreText();
        }

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

    void UpdatePlayerScoreText()
    {
        player1ScoreText.text = "" + numberOfPlayerOneWins;
        player2ScoreText.text = "" + numberOfPlayerTwoWins;
    }


    public bool AddTokenToBoard(byte index)
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
            return false;
        }

        moveMade = true;
        spacesRemaining--;
        return true;
    }

    bool Player1Wins()
    {
        // Check all rows.
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

        // Check all columns.
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

        // Check the diagonals.
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

    bool Player2Wins()
    {
        // Check all rows.
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

        // Check all columns.
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

        // Check the diagonals.
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

    void OpenResultsPopup(byte messageIndex)
    {
        popupText.text = (messageIndex == 0) ? "We have a draw..." : "Player " + messageIndex + " wins!";
    }

    void RespondToOKButtonOnPopup()
    {
        popupPanel.SetActive(false);
    }

}
