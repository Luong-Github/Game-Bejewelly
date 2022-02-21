using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySecond;
    public GameObject hintPartice;
    public GameObject currentHint;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySecond = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelaySecond -= Time.deltaTime;
        if(hintDelaySecond <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySecond = hintDelay;
        }
    }

    // first, find all posible matches on board
    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMatches = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMatches.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMatches.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMatches;
    }
    // pick one of those matches randomly
    GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if(possibleMoves.Count > 0)
        {
            int pieceToMove = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToMove];
        }
        return null;
    }
    // create  the hint behind the chosen match
    private void MarkHint()
    {
        GameObject move = PickOneRandomly();
        if(move != null)
        {
            currentHint = Instantiate(hintPartice, move.transform.position, Quaternion.identity);

        }
    }
    // destroy the hint
    public void DestroyHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySecond = hintDelay;
        }
    }
}
