using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="World", menuName ="Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int width;
    public int height;

    [Header("Starting tiles")]
    public TileType[] boardLayout; 

    [Header("Available Dots")]
    public GameObject[] dots;

    [Header("Score Goals")]
    public int[] scoreGoals;

    [Header("End Game Requirements")]
    public EndGameRequirement endGameRequirement;
    public BlankGoal[] levelGoals;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
