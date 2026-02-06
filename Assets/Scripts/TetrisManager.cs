using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TetrisManager : MonoBehaviour
{
    //needs to be accessable by other stupid pieces of stupid game code
    public int score { get; private set; }
    public bool gameOver { get; private set; }

    public bool gameVictory { get; private set; }

    public UnityEvent OnScoreChanged;
    public UnityEvent OnGameOver;


    private void Start()
    {
      SetGameOver(false, false);
    }

    public int calculateScore(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }

    public void ChangeScore(int amount)
    {
        //future considersy? perchance amount be negative?
        score += amount;
        OnScoreChanged.Invoke();
    }

    public void SetGameOver(bool gameOver, bool win)
    {
        if (!gameOver)
        {
            //if gameover event is FALSE reset the rootin tootin score
            score = 0;
            ChangeScore(0);
        }
        gameVictory = win;
        this.gameOver = gameOver;
        OnGameOver.Invoke();
    }
}
