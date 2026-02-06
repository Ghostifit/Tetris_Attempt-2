using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public TetrisManager tetrisManager;
    public TextMeshProUGUI scoreText;
    public GameObject endGamePanel;
    public GameObject GameVictory; 

    public void UIUpdateScore()
    {
        scoreText.text = $"SCORE: {tetrisManager.score}";
    }

    public void UpdateGameOver()
    {
        //when the silly game over event is brodcasted the silly end game
        //panel will show and then will hide when the silly lil game resets
        GameVictory.SetActive(tetrisManager.gameVictory && tetrisManager.gameOver);
        endGamePanel.SetActive(!tetrisManager.gameVictory && tetrisManager.gameOver);
    }
    public void UpdateGameVictory()
    {
       
    }

    public void PlayAgain()
    {
        //resets the silly lil game
        tetrisManager.SetGameOver(false, false);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
