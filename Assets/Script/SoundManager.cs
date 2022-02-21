using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource click;
    public AudioSource swipeNoise;
    public AudioSource startNoise;

     

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 0)
            {
                startNoise.Play();
                startNoise.volume = 0;
            }
            else
            {
                startNoise.Play();
                startNoise.volume = 1;
            }
        }
        else
        {
            startNoise.Play();
            startNoise.volume = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void adjustVoice()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                startNoise.volume = 0;
            }
            else
            {
                startNoise.volume = 1;
            }
        }
    }

    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 1)
            {
                int clipToPlay = Random.Range(0, destroyNoise.Length);
                // play that clip
                destroyNoise[clipToPlay].Play();
            }
        }
        else
        {
            int clipToPlay = Random.Range(0, destroyNoise.Length);
            // play that clip
            destroyNoise[clipToPlay].Play();
        }

    }

    public void PlayClickNoise()
    {
        click.Play();
    }

    public void PlaySwipeNoise()
    {
        swipeNoise.Play();
    }
}
