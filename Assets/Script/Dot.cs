using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    [Header("Board Variables")]
    public int columns;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDots;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    [Header("Sound")]
    private SoundManager soundManager;

    [Header("Swipe Stuff")]
    public float swipeAngel = 0;
    public float swipeResist = 1f;

    [Header("Powerup Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public GameObject adjacentMaker;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;

    public List<GameObject> gameObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        board = GameObject.FindWithTag("Board").GetComponent<Board>();// faster than find objectoftype
        //board = FindObjectOfType<Board>();  very slow
        endGameManager = FindObjectOfType<EndGameManager>();
        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();
        soundManager = FindObjectOfType<SoundManager>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //columns = targetX;
        //previousRow = row;
        //previousColumn = columns;
    }

    // This is for testing and debug
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject maker = Instantiate(colorBomb, transform.position, Quaternion.identity);
            maker.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            Color currentColor = mySprite.color;
            mySprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, .5f);
        }
        targetX = columns;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move toward the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
            if(board.allDots[columns, row] != this.gameObject)
            {
                board.allDots[columns, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move toward the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
            if (board.allDots[columns, row] != this.gameObject)
            {
                board.allDots[columns, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
            
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    /// <summary>
    /// Check not matched dot, and return previous dot position
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckMoveCo()
    {
        if (isColorBomb)
        {
            //this pieces is color bomb, and other pieces is the color to destroy
            findMatches.MatchPieceOfColor(otherDots.tag);
            //findMatches.DrawLineToSameColor(otherDots.tag, otherDots);
            //Debug.DrawLine(Vector3.zero, new Vector3(20, 20, 0), Color.red);
            isMatched = true;
            GameObject temp = Instantiate(findMatches.colorBombPartice, otherDots.transform.position, Quaternion.identity);
            Destroy(temp, .5f);
        }
        else if(otherDots.GetComponent<Dot>().isColorBomb)
        {
            // the other piece is a color bomb, and this piece has color bomb to destroy
            findMatches.MatchPieceOfColor(this.gameObject.tag);
            //findMatches.DrawLineToSameColor(this.gameObject.tag, this.gameObject);
            //Debug.DrawLine(Vector3.zero, new Vector3(20, 20, 0), Color.red);
            otherDots.GetComponent<Dot>().isMatched = true;
            GameObject temp = Instantiate(findMatches.colorBombPartice, this.gameObject.transform.position, Quaternion.identity);
            Destroy(temp, .5f);
        }
        yield return new WaitForSeconds(.5f);
        if(otherDots != null)
        {
            if (!isMatched && !otherDots.GetComponent<Dot>().isMatched)
            {
                // move them back
                otherDots.GetComponent<Dot>().row = row;
                otherDots.GetComponent<Dot>().columns = columns;
                row = previousRow;
                columns = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                if(endGameManager != null)
                {
                    if(endGameManager.endGameRequirement.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();
            }
            //otherDots = null;
        }
        
    }

    private void OnMouseDown()
    {
        //Destroy hint
        if(hintManager != null)
        {
            hintManager.DestroyHint();
        }
        soundManager.PlayClickNoise();
        if(board.currentState == GameState.move)
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    private void OnMouseUp()
    {

        if(board.currentState == GameState.move)
        {
            soundManager.PlaySwipeNoise();
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
        
    }

    /// <summary>
    /// calculate angel for swiping, 
    /// </summary>
    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || 
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeAngel)
        {
            board.currentState = GameState.wait;
            swipeAngel = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg; // convert radian to degrees
            MovePiece();
            
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
            
        }
    }

    void MovePieceActual(Vector2 direction )
    {
        otherDots = board.allDots[columns + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = columns;
        if (board.lockTitle[columns, row] == null
            && board.lockTitle[columns + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherDots != null)
            {


                otherDots.GetComponent<Dot>().columns += -1 * (int)direction.x;
                otherDots.GetComponent<Dot>().row += -1 * (int)direction.y;
                columns += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = GameState.move;
            }
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    /// <summary>
    /// Move dot based on angel
    /// </summary>
    void MovePiece()
    {
        // swipe on the right
        if (swipeAngel > -45 && swipeAngel <= 45 && columns < board.width - 1)
        {
            //right swipe
            //otherDots = board.allDots[columns + 1, row];
            //previousRow = row;
            //previousColumn = columns;
            //otherDots.GetComponent<Dot>().columns -= 1;
            //columns += 1;
            //StartCoroutine(CheckMoveCo());
            
            MovePieceActual(Vector2.right);
        }
        else if (swipeAngel > 45 && swipeAngel <= 135 && row < board.height - 1)
        {
            // top swipe
            //otherDots = board.allDots[columns, row + 1];
            //previousRow = row;
            //previousColumn = columns;
            //otherDots.GetComponent<Dot>().row -= 1;
            //row += 1;
            //StartCoroutine(CheckMoveCo());
            
            MovePieceActual(Vector2.up);
        }
        else if ((swipeAngel > 135 || swipeAngel <= -135) && columns > 0)
        {
            // left swipe
            //otherDots = board.allDots[columns - 1, row];
            //previousRow = row;
            //previousColumn = columns;
            //otherDots.GetComponent<Dot>().columns += 1;
            //columns -= 1;
            //StartCoroutine(CheckMoveCo());
            
            MovePieceActual(Vector2.left);
        }
        else if (swipeAngel < -45 && swipeAngel >= -135 && row > 0)
        {
            // down swipe
            //otherDots = board.allDots[columns, row - 1];
            //previousRow = row;
            //previousColumn = columns;
            //otherDots.GetComponent<Dot>().row += 1;
            //row -= 1;
            //StartCoroutine(CheckMoveCo());
            
            MovePieceActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move; 
        }
        
    }



    /// <summary>
    /// set matched dot  in 4 direction based on tag name
    /// </summary>
    void FindMatches()
    {
        GameObject currentObject = this.gameObject;
        if(columns > 0 && columns < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[columns - 1, row];
            GameObject rightDot1 = board.allDots[columns + 1, row];
            if(leftDot1 != null && rightDot1 != null)
                if(leftDot1.tag == currentObject.tag && rightDot1.tag == currentObject.tag)
                {
                    if (!gameObjects.Contains(leftDot1))
                    {
                        gameObjects.Add(leftDot1);
                    }
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    if (!gameObjects.Contains(rightDot1))
                    {
                        gameObjects.Add(rightDot1);
                    }
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    if (!gameObjects.Contains(currentObject))
                    {
                        gameObjects.Add(currentObject);
                    }
                    isMatched = true;
                }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[columns, row + 1];
            GameObject downDot1 = board.allDots[columns, row - 1];
            if(upDot1 != null && downDot1 != null)
                if (upDot1.tag == currentObject.tag && downDot1.tag == currentObject.tag)
                {
                    if (!gameObjects.Contains(upDot1))
                    {
                        gameObjects.Add(upDot1);
                    }
                    upDot1.GetComponent<Dot>().isMatched = true;
                    if (!gameObjects.Contains(downDot1))
                    {
                        gameObjects.Add(downDot1);
                    }
                    downDot1.GetComponent<Dot>().isMatched = true;
                    if (!gameObjects.Contains(currentObject))
                    {
                        gameObjects.Add(currentObject);
                    }
                    isMatched = true;
                }
        }
    }

    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isAdjacentBomb && !isColorBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isAdjacentBomb && !isColorBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColorBomb()
    {
        if (!isColumnBomb && !isAdjacentBomb && !isRowBomb)
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            GameObject maker = Instantiate(adjacentMaker, transform.position, Quaternion.identity);
            maker.transform.parent = this.transform;
        }
    }
}
