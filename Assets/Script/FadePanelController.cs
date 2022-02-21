using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnimator;
    public Animator gameInforAnim;
    public Animator tryAgainAnim;
    public Animator youWinAnimator;
    public void OK()
    {
        if(panelAnimator  != null && gameInforAnim != null)
        {
            panelAnimator.SetBool("out", true);
            gameInforAnim.SetBool("out", true);
            StartCoroutine(GameStartCo());
        }
        
    }

    public void GameOver()
    {
        panelAnimator.SetBool("out", false);
        panelAnimator.SetBool("GameOVer", true);
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
