using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string LevelToLoad;
    public int level;
    private GameData gameData;
    private int highScore;


    [Header("UI stuff")]
    public Image[] stars;
    public Text HighScoreText;
    public Text starText;
    private int starsActive;
    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
    }

    public void LoadData()
    {
        if(gameData != null)
        {
            starsActive = gameData.saveData.stars[level];
            highScore = gameData.saveData.highScores[level];
        }
    }

    void SetText()
    {
        HighScoreText.text = "" + highScore;
        starText.text = "" + starsActive + "/3";
    }

    void ActivateStars()
    {

        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level);
        SceneManager.LoadScene(LevelToLoad);
    }

}
