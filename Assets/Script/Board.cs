using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal,
    Lock,
    Concrete,
    Slime
}

[System.Serializable]
public class TileType
{
    public int x; 
    public int y;
    public TileKind tileKind;
}
public class Board : MonoBehaviour
{
    [Header("Scriptable Objects Stuff")]
    public World world;
    public int level;
    public GameState currentState = GameState.move;

    [Header("Board Dimensions")]
    public int width;
    public int height;
    public int offSet;

    [Header("Prefabs")]
    public GameObject prefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject concretePrefab;
    public GameObject slimePiecePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    [Header("Layout")]
    public TileType[] boardLayout;
    private bool[,] allCells;
    private TitleBackground[,] breakableTile;
    public TitleBackground[,] lockTitle;
    public TitleBackground[,] concreteTitle;
    public TitleBackground[,] slimeTitle;
    public GameObject[,] allDots;

    [Header("Match Stuff")]
    public MatchType matchType;
    public Dot currentDot;
    private FindMatches findMachtes;
    public int basePieceValue = 20;
    public int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;
    private bool makeSlime = true;
    private int refillBoardToMakeSlime = 0;

    private void Awake()
    {
        refillBoardToMakeSlime = 0;
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }

        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        slimeTitle = new TitleBackground[width, height];
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        breakableTile = new TitleBackground[width, height];
        lockTitle = new TitleBackground[width, height];
        concreteTitle = new TitleBackground[width, height];
        findMachtes = FindObjectOfType<FindMatches>();
        allCells = new bool[width, height];
        allDots = new GameObject[width, height];
        setUp();
        currentState = GameState.pause;
        refillBoardToMakeSlime = 0;
    }

    public void GenerateBlankSpace()
    {
        for(int i = 0; i< boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind == TileKind.Blank)
            {
                allCells[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTile()
    {
        // look at all the tiles in the layout
        for(int i = 0; i< boardLayout.Length; i++)
        {
            // if a tile is "Jelly" tile
            if(boardLayout[i].tileKind == TileKind.Breakable)
            {
                // create a "Jelly" tile
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTile[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<TitleBackground>();
            }
        }
    }

    private void GenerateLockTiles()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind == TileKind.Lock)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTitle[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<TitleBackground>();
            }
        }
    }

    private void GenerateSlimeTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Slime)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(slimePiecePrefab, tempPosition, Quaternion.identity);
                slimeTitle[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<TitleBackground>();
            }
        }
    }

    private void GenerateConcreteTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Concrete)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concretePrefab, tempPosition, Quaternion.identity);
                concreteTitle[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<TitleBackground>();
            }
        }
    }

    private void setUp()
    { 
        GenerateBlankSpace();
        GenerateBreakableTile();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!allCells[i, j] && !concreteTitle[i,j] && !slimeTitle[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundCell = Instantiate(prefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundCell.transform.parent = this.transform;
                    backgroundCell.name = "(" + i + "," + j + ")";
                    int dotToUse = Random.Range(0, dots.Length);

                    // check matched dot at initiation
                    int maxIteration = 0;
                    while (MatchedAt(i, j, dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIteration++;
                    }
                    maxIteration = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().columns = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>    
    private bool MatchedAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag
                && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) { 
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // return what kind of bomb to make
    private int ColumnOrRow()
    {
        
        // Make a copy of current matches 
        List<GameObject> matchCopy = findMachtes.currentMatches as List<GameObject>;

        // cycle through all of match Copy and decide if a bomb needs to be made
        for(int i = 0; i < matchCopy.Count; i++)
        {
            // store 
            Dot thisDot = matchCopy[i].GetComponent<Dot>();

            // 
            int column = thisDot.columns;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;
            // Cycle through the rest of dot and compare
            for(int j = 0; j < matchCopy.Count; j++)
            {
                // store the next dot
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if(nextDot == thisDot)
                {
                    continue; 
                }

                if(nextDot.columns == thisDot.columns && nextDot.CompareTag(thisDot.tag))
                {
                    columnMatch++;
                }

                if (nextDot.row == thisDot.row && nextDot.CompareTag(thisDot.tag))
                {
                    rowMatch++;
                }
            }

            // return 3 if column or row match
            if(columnMatch == 4 || rowMatch == 4)
            {
                return 1;
            }


            // retunr 2 if adjacent bomb
            if(columnMatch == 2 || rowMatch == 2)
            {
                return 2;
            }
            // return 1 if color bomb
            if (columnMatch == 3 || rowMatch == 3)
            {
                return 3;
            }
            // r
        }

        return 0;
    }

    private void CheckToMakeBomb()
    {
        // how many objects are in findmatches currentmathces
        if (findMachtes.currentMatches.Count > 3)
        { 
            // how many matchs
            int typeOfMatch = ColumnOrRow();
            if (typeOfMatch == 1)
            {
                //Make color bomb
                // is current dot matched
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDots != null)
                        {
                            Dot otherDot = currentDot.otherDots.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }

            }
            else if (typeOfMatch == 2)
            {
                // Make adjacent bomb
                // is current dot matched
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDots != null)
                        {
                            Dot otherDot = currentDot.otherDots.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
            else if(typeOfMatch == 3)
            {
                findMachtes.CheckBomb();
            }
        } 
    }

    public void BombRow(int row)
    {
        for(int i = 0; i < width; i++)
        {
                if(concreteTitle[i, row])
                {
                    concreteTitle[i, row].TakeDamage(1);
                    if(concreteTitle[i, row].hitPoints <= 0)
                    {
                        concreteTitle[i, row] = null;
                    }
                }
        }

    }

    public void BombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        {
                if (concreteTitle[column, i])
                {
                    concreteTitle[column, i].TakeDamage(1);
                    if (concreteTitle[column, i].hitPoints <= 0)
                    {
                        concreteTitle[column, i] = null;
                    }
                }
        }
    }

    /// <summary>
    /// Destroy at matched dot 
    /// </summary>
    private void DestroyMatchesAt(int column, int row)
    {
        //
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            if(breakableTile[column, row] != null)
            {
                //if it does, give one damage
                breakableTile[column, row].TakeDamage(1);
                if(breakableTile[column, row].hitPoints <= 0)
                {
                    breakableTile[column, row] = null;
                }
            }

            if (lockTitle[column, row] != null)
            {
                //if it does, give one damage
                lockTitle[column, row].TakeDamage(1);
                if (lockTitle[column, row].hitPoints <= 0)
                {
                    lockTitle[column, row] = null;
                }
            }

            DamageConcrete(column, row);
            DamageSlime(column, row);
            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoal();
            }

            if(soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();
            }
            //findMachtes.currentMatches.Remove(allDots[column, row]);  
            
            GameObject currentInstantie = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(currentInstantie, .2f);

            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    private void DamageConcrete(int column, int row)
    {
        // left
        if(column > 0)
        {
            if(concreteTitle[column - 1, row])
            {
                concreteTitle[column - 1, row].TakeDamage(1);
                if (concreteTitle[column - 1, row].hitPoints <= 0)
                {
                    concreteTitle[column - 1, row] = null;
                }
            }
        }

        //right
        if (column < width - 1)
        {
            if (concreteTitle[column + 1, row])
            {
                concreteTitle[column + 1, row].TakeDamage(1);
                if (concreteTitle[column + 1, row].hitPoints <= 0)
                {
                    concreteTitle[column + 1, row] = null;
                }
            }
        }

        if (row > 0)
        {
            if (concreteTitle[column, row - 1])
            {
                concreteTitle[column, row - 1].TakeDamage(1);
                if (concreteTitle[column, row - 1].hitPoints <= 0)
                {
                    concreteTitle[column, row - 1] = null;
                }
            }
        }

        if (row < height - 1)
        {
            if (concreteTitle[column , row + 1])
            {
                concreteTitle[column, row + 1].TakeDamage(1);
                if (concreteTitle[column, row + 1].hitPoints <= 0)
                {
                    concreteTitle[column, row + 1] = null;
                }
            }
        }
    }

    private void DamageSlime(int column, int row)
    {
        // left
        if (column > 0)
        {
            if (slimeTitle[column - 1, row])
            {
                slimeTitle[column - 1, row].TakeDamage(1);
                if (slimeTitle[column - 1, row].hitPoints <= 0)
                {
                    slimeTitle[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }

        //right
        if (column < width - 1)
        {
            if (slimeTitle[column + 1, row])
            {
                slimeTitle[column + 1, row].TakeDamage(1);
                if (slimeTitle[column + 1, row].hitPoints <= 0)
                {
                    slimeTitle[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }

        if (row > 0)
        {
            if (slimeTitle[column, row - 1])
            {
                slimeTitle[column, row - 1].TakeDamage(1);
                if (slimeTitle[column, row - 1].hitPoints <= 0)
                {
                    slimeTitle[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }

        if (row < height - 1)
        {
            if (slimeTitle[column, row + 1])
            {
                slimeTitle[column, row + 1].TakeDamage(1);
                if (slimeTitle[column, row + 1].hitPoints <= 0)
                {
                    slimeTitle[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
    }

    public void DestroyMatches() 
    {
        if (findMachtes.currentMatches.Count >= 4)
        {
            CheckToMakeBomb();
        }
        findMachtes.currentMatches.Clear();
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                // if the current spot isn't blank or empty...
                if(!allCells[i,j] && allDots[i,j] == null && !concreteTitle[i,j] && !slimeTitle[i,j])
                {
                    for (int k = j + 1;k < height; k++)
                    {
                        // if a dot is found
                        if(allDots[i,k] != null)
                        {
                            // move that dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            // set that spot to be null
                            allDots[i, k] = null;
                            // break the loop
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());

    }
    /// <summary>
    /// Collapsing Columns
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    /// <summary>
    /// Refilling the board after collapsing 
    /// </summary>
    private void RefillBoard()
    {
        
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null && !allCells[i,j] && !concreteTitle[i,j] && !slimeTitle[i,j])
                {
                    Vector2 tempPosition = new Vector2(i,j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while(MatchedAt(i ,j , dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i,j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().columns = i;
                }
            }
        }
    }

    /// <summary>
    /// check matches on board when refilling board
    /// </summary>
    /// <returns></returns>
    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Coroutine to run refillboard
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillBoardCo()
    {
        
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            //yield return new WaitForSeconds(2 * refillDelay);
        }
        //findMachtes.currentMatches.Clear();
        currentDot = null;
        CheckToMakeSlime();
        //yield return new WaitForSeconds(.5f);
        if (IsDeadLocker())
        {
            StartCoroutine(ShuffleBoard());
            Debug.Log("DeadLocked!!!");
        }
        System.GC.Collect();
        if (currentState != GameState.pause)
        {
            currentState = GameState.move;
        }
        if(refillBoardToMakeSlime != 1)
            makeSlime = true;
        
        streakValue = 1;
    }

    private void CheckToMakeSlime()
    {
        // 
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(slimeTitle[i,j] != null && makeSlime)
                {
                    // call new slime
                    MakeNewSlime();
                }
            }
        }

        refillBoardToMakeSlime = 0;
    }

    private Vector2 CheckForAdjacent(int column, int row)
    {
        if (column < width - 1 && allDots[column + 1, row])
        {
            return Vector2.right;
        }
        if (column > 0 && allDots[column - 1, row])
        {
            return Vector2.left;
        }
        if (row < height - 1 && allDots[column, row + 1])
        {
            return Vector2.up;
        }
        if (row > 0 && allDots[column, row - 1])
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }
    //private Vector2 CheckForAdjacent(int column, int row)
    //{
    //    Debug.Log(column + " " + column + 1);
    //    if(allDots[column + 1, row] && column < width - 1)
    //    {
    //        return Vector2.right;
    //    }
    //    if (allDots[column - 1, row] && column > 0)
    //    {
    //        return Vector2.left;
    //    }
    //    if (allDots[column , row + 1] && column < height - 1)
    //    {
    //        return Vector2.up;
    //    }
    //    if (allDots[column , row- 1] && column > 0)
    //    {
    //        return Vector2.down;
    //    }

    //    return Vector2.zero;
    //}

    private void MakeNewSlime()
    {
        if(refillBoardToMakeSlime == 1)
        {
            return;
        }
        Debug.Log("Make Slime");
        bool slime = false;
        int loops = 0;
        while (!slime && loops < 50)
        {
            int newX = Random.Range(0, width);

            int newY = Random.Range(0, height);
            if(slimeTitle[newX, newY])
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                if(adjacent != Vector2.zero)
                {
                    refillBoardToMakeSlime = 1;
                    Destroy(allDots[newX + (int)adjacent.x, newY + (int)adjacent.y]);
                    Vector2 temp = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    GameObject tile = Instantiate(slimePiecePrefab, temp, Quaternion.identity);
                    slimeTitle[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<TitleBackground>();
                    slime = true;
                    break;
                }
            }
            loops++;
        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            // Take the second piece and save it in a holder
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            // switching the first dot to be 
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            // set the first dot to be 
            allDots[column, row] = holder;
        }
    }

    private bool CheckForMatches()
    { 
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] != null)
                {
                    // make sure one or two to the right are in the board
                    if (i < width - 2)
                    {
                        // Check if the dots to the right and two 
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                                && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                                && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }

        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocker()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(.5f);
        // create a game object
        List<GameObject> newBoard = new List<GameObject>();
        // add every piece to this board
        for (int i = 0; i < width; i++)
        {
            for( int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        yield return new WaitForSeconds(.5f);
        // for every spot on new board
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                // if this spot shouldn't be blank space
                if (!allCells[i, j] && !concreteTitle[i,j] && !slimeTitle[i,j])
                {
                    // pick random number
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    // make a container for the piece

                    
                    // asign column and row to piece
                    int maxIteration = 0;
                    while (MatchedAt(i, j, newBoard[pieceToUse]) && maxIteration < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIteration++;
                    }
                    maxIteration = 0;

                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    piece.columns = i;
                    piece.row = j;
                    // fill in the dots array with this new piece
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        // if deadlock 
        if (IsDeadLocker())
        {
            ShuffleBoard();
        }


    }

}
