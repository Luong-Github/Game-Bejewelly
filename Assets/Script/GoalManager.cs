using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeed;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>(); 
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    private Board board;
    private EndGameManager endGame;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        endGame = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupGoals();
    }

    void GetGoals()
    {
        if(board != null)
        {
            if(board.world != null)
            {
                if (board.level < board.world.levels.Length)
                {
                    if (board.world.levels[board.level] != null)
                    {
                        levelGoals = board.world.levels[board.level].levelGoals;
                        for(int i = 0; i < levelGoals.Length; i++)
                        {
                            levelGoals[i].numberCollected = 0;
                        }
                    }
                }
            }
        }
    }

    void SetupGoals()
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            //Create a new Goal Panel at the goalIntroParent position
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goalIntroParent.transform.localScale.Set(1, 1, 1);
            goalIntroParent.transform.lossyScale.Set(1, 1, 1);
            goal.transform.SetParent(goalIntroParent.transform);
            // set the image in text of goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeed;


            //Create a new Goal Panel at the goalGameParent position
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeed;
        }
    }

    public void UpdateGoal()
    {
        int goalsCompleted = 0;
        for(int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeed;
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeed)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeed + "/" + levelGoals[i].numberNeed;
            }
        }

        if(goalsCompleted >= levelGoals.Length)
        {
            if(endGame != null)
            {
                endGame.WinGame();
            }
            Debug.Log("You win!!");
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            if(goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
