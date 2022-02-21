using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirement
{
    // what kind of game 
    public GameType gameType;
    public int counterValue;

}

public class EndGameManager : MonoBehaviour
{
    
    public GameObject moveObject,timeObject;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counterObject;
    public EndGameRequirement endGameRequirement;
    public int currentCounterValue;
    private Board board;
    private float timerSecond;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetupGame();
    }

    void setGameType()
    {
        if(board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    endGameRequirement = board.world.levels[board.level].endGameRequirement;
                }
            }
        }
    }

    void SetupGame()
    {



        currentCounterValue = endGameRequirement.counterValue;
        if(endGameRequirement.gameType == GameType.Moves)
        {
            moveObject.SetActive(true);
            timeObject.SetActive(false);
        }
        else
        {
            timerSecond = 1;

            moveObject.SetActive(false);
            timeObject.SetActive(true);
        }
        counterObject.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counterObject.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counterObject.text = "" + currentCounterValue;
        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        fadePanel.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("End game");
        currentCounterValue = 0;
        counterObject.text = "" + currentCounterValue;
        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        fadePanel.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if(endGameRequirement.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSecond -= Time.deltaTime;
            if(timerSecond <= 0)
            {
                DecreaseCounterValue();
                timerSecond = 1;
            }
        }
    }
}
