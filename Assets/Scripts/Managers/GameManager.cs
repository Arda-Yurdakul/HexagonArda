using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Running,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private int hexScoreValue;

    Board board;
    private int score;
    private int moves;
    private int highScore;
    private int bombTarget;

    public GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Running;
        board = FindObjectOfType<Board>();
        score = 0;
        moves = 0;
        bombTarget = 10;
        highScore = PlayerPrefs.GetInt("highScore");
        Invoke("UpdateHUD", 0.25f);
    }

    private void UpdateHUD()
    {
        UIManager.Instance.UpdateMovesText(0);
        UIManager.Instance.UpdateHighScoreText(highScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore()
    {
        score += hexScoreValue;
        UIManager.Instance.UpdateScoreText(score);
        TryUpdateHighScore();
        TryBombSpawn();
    }

    
    private void TryBombSpawn()
    {
        if(score >= bombTarget)
        {
            bombTarget += 1000;
            board.SpawnBomb();
        }
    }

    public void IncreaseMoves()
    {
        moves++;
        UIManager.Instance.UpdateMovesText(moves);
    }

    public void TryUpdateHighScore()
    {
        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            UIManager.Instance.UpdateHighScoreText(highScore);
        }
    }

    internal void EndGame()
    {
        gameState = GameState.GameOver;
        board.ClearBoard();
        StartCoroutine(UIManager.Instance.ActivateGameOverPanel());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }

}
