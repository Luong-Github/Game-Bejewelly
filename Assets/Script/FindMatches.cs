using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    public LineRenderer lineRenderer;
    public Transform[] points;

    public int MatchDelayp;
    public GameObject columnBombPartice;
    public GameObject rowBombPartice;
    public GameObject colorBombPartice;
    public float animDelay = 3;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.columns, dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.columns, dot2.row));
        }
        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.columns, dot3.row));
        }
        return currentDots;
    }

    /// <summary>
    /// create find all matches coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i< board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if(currentDot != null)
                {
                    Dot dot = currentDot.GetComponent<Dot>();
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if(leftDot != null && rightDot != null)
                        {
                            Dot leftToDot = leftDot.GetComponent<Dot>();
                            Dot rightToDot = rightDot.GetComponent<Dot>();
                            if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isRowBomb 
                                    || leftDot.GetComponent<Dot>().isRowBomb ||
                                    rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    // union two list
                                    currentMatches.Union(GetRowPieces(j));

                                }

                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                    board.BombColumn(currentDot.GetComponent<Dot>().columns);
                                    GameObject temp = Instantiate(columnBombPartice, currentDot.transform.position, Quaternion.identity);
                                    Destroy(temp, 0.5f);
                                    Debug.Log(temp.tag);
                                }

                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                    board.BombColumn(leftDot.GetComponent<Dot>().columns);
                                    GameObject temp = Instantiate(columnBombPartice, leftDot.transform.position, Quaternion.identity);
                                    Destroy(temp, 0.5f);
                                    Debug.Log(temp.tag);
                                }

                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                    board.BombColumn(rightDot.GetComponent<Dot>().columns);
                                    GameObject temp = Instantiate(columnBombPartice, rightDot.transform.position, Quaternion.identity);
                                    
                                    Destroy(temp, 0.5f);
                                    Debug.Log(temp.tag);
                                }

                                currentMatches.Union(IsAdjacentBomb(leftToDot, dot, rightToDot));

                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rightDot))
                                {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {

                        GameObject upDot = board.allDots[i , j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            Dot upToDot = upDot.GetComponent<Dot>();
                            Dot downToDot = downDot.GetComponent<Dot>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isColumnBomb
                                    || upDot.GetComponent<Dot>().isColumnBomb ||
                                    downDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    // union two list
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                    board.BombRow(currentDot.GetComponent<Dot>().row);
                                    GameObject temp = Instantiate(rowBombPartice, currentDot.transform.position, Quaternion.identity);
                                    Destroy(temp, .5f);
                                    Debug.Log(temp.tag);
                                }

                                if (upDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                    board.BombRow(upDot.GetComponent<Dot>().row);
                                    GameObject temp = Instantiate(rowBombPartice, upDot.transform.position, Quaternion.identity);
                                    Destroy(temp, .5f);
                                    Debug.Log(temp.tag);
                                }

                                if (downDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                    board.BombRow(downDot.GetComponent<Dot>().row);
                                    GameObject temp = Instantiate(rowBombPartice, downDot.transform.position, Quaternion.identity);
                                    Destroy(temp, .5f);
                                    Debug.Log(temp.tag);
                                }

                                currentMatches.Union(IsAdjacentBomb(upToDot, dot, downToDot));

                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(downDot))
                                {
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
    
    public void MatchPieceOfColor(string color)
    {
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                // check if the piece exists
                if(board.allDots[i,j] != null)
                {
                    if(board.allDots[i,j].tag == color)
                    {
                        // set dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                        
                    }
                }
            }
        }
    }


    public void DrawLineToSameColor(string color, GameObject dot)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // check if the piece exists
                if (board.allDots[i, j] != null)
                {
                    if (board.allDots[i, j].tag == color)
                    {
                        var dist = Vector3.Distance(dot.transform.position, board.allDots[i, j].transform.position);
                        Debug.Log(dist);

                        lineRenderer.SetPosition(0, dot.transform.position);
                        lineRenderer.SetWidth(.45f, .45f);

                        Vector3 length = Vector3.Normalize(board.allDots[i, j].transform.position - dot.transform.position) + dot.transform.position;
                        lineRenderer.SetPosition(1, length);


                    }
                }
            }
        }
    }


    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = column - 1; i <= column + 1; i++)
        {
            for(int j = row - 1; j <= row + 1; j++)
            {
                // check if piece is inside the board
                if (i >= 0 && i < board.width && j >= 0 && j <= board.height)
                {
                    if(board.allDots[i,j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i,j].GetComponent<Dot>().isMatched = true;
                    }

                }
            }
        }
        return dots;
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if(board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBomb()
    {
        // Did the player move something?
        if(board.currentDot != null)
        {
            // Is the piece they moved matched?
            if (board.currentDot.isMatched)
            {
                // make it unmatched 
                board.currentDot.isMatched = false;
                // Decide what kind of bomb to make
                if((board.currentDot.swipeAngel > -45 && board.currentDot.swipeAngel <= 45)
                    || (board.currentDot.swipeAngel < -135 && board.currentDot.swipeAngel >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            // Is the other piece matched?
            else if (board.currentDot.otherDots != null)
            {
                Dot otherDot = board.currentDot.otherDots.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    // make it unmatched
                    otherDot.isMatched = false;
                    // Decide what kind of bomb to make
                    if ((board.currentDot.swipeAngel > -45 && board.currentDot.swipeAngel <= 45)
                        || (board.currentDot.swipeAngel < -135 && board.currentDot.swipeAngel >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }

}
