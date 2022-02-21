using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseObject;
    
    private Board board;
    public bool pause = false;
    public Image soundButton;
    public Sprite musicOn;
    public Sprite musicOff;
    private SoundManager sound;
    // Start is called before the first frame update
    void Start()
    {
        
        sound = FindObjectOfType<SoundManager>();
        if (PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOff;
            }
            else 
            {
                soundButton.sprite = musicOn;
            }
        }
        else
        {
            soundButton.sprite = musicOn;
        }
        if(pauseObject != null)
        {
            pauseObject.SetActive(false);
        }
        
        board = GameObject.FindWithTag("Board").GetComponent<Board>();

    }

    public void PauseGame()
    {
        pause = !pause;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(pause && !pauseObject.activeInHierarchy)
        {
            pauseObject.SetActive(true);
            board.currentState = GameState.pause;
        }
        if (pauseObject != null)
        {
            if (pauseObject.activeInHierarchy && !pause)
            {
                pauseObject.SetActive(false);
                board.currentState = GameState.move;
            }
        }
    }

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOn;
                PlayerPrefs.SetInt("Sound", 1);
                sound.adjustVoice();
            }
            else
            {
                soundButton.sprite = musicOff;
                PlayerPrefs.SetInt("Sound", 0);
                sound.adjustVoice();
            }
        }
        else
        {
            soundButton.sprite = musicOff;
            PlayerPrefs.SetInt("Sound", 1);
            sound.adjustVoice();
        }
    }

    public void ExitGame()
    {
        GameStartManager.count = 1;
        SceneManager.LoadScene("Splash");
        
    }
}
