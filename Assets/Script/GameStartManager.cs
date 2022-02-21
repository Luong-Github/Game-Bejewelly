using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameStartManager : MonoBehaviour
{ 
    public GameObject startPanel;
    public GameObject levelPanel;
    public GameObject menuPanel;
    public GameObject optionPanel;
    public GameObject helpPanel;
    public GameData gameData;
    public SoundManager sound;
    //public static GameStartManager startManager;
    public static int count = 0;
    public void wake()
    {
        //startManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        sound = FindObjectOfType<SoundManager>();
        if(count == 0)
        {
            startPanel.SetActive(true);
            levelPanel.SetActive(false);
        }
        else
        {
            startPanel.SetActive(false);
            levelPanel.SetActive(true);
        }
        menuPanel.SetActive(false);
        optionPanel.SetActive(false);
        helpPanel.SetActive(false);
    }

    public void NextToLevelPanel()
    {
        sound.PlayClickNoise();
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void NextToOptionPanel()
    {
        sound.PlayClickNoise();
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void NextToHelpPanel()
    {
        sound.PlayClickNoise();
        menuPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    public void StartGame()
    {
        sound.PlayClickNoise();
        menuPanel.SetActive(true);
        startPanel.SetActive(false);
        
    }

    public void QuitGame()
    {
        sound.PlayClickNoise();
        Application.Quit();
    }
    public void Home()
    {
        sound.PlayClickNoise();
        startPanel.SetActive(false);
        levelPanel.SetActive(false);
        optionPanel.SetActive(false);
        menuPanel.SetActive(true);
        helpPanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
