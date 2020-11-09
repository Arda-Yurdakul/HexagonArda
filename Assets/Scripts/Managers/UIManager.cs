using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Simple Singleton class for updating the UI elements when necessary
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            return null;
        }
    }

    Text scoreText;
    Text highScoreText;
    Text moveText;
    GameObject gameOverPanel;

   

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        Transform HUDPanel = transform.GetChild(0).GetChild(1);
        gameOverPanel = transform.GetChild(0).GetChild(2).gameObject;
        gameOverPanel.SetActive(false);
        scoreText = HUDPanel.Find("ScoreText").GetComponent<Text>();
        highScoreText = HUDPanel.Find("HiScoreText").GetComponent<Text>();
        moveText = HUDPanel.GetChild(1).Find("MovesText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateHighScoreText(int score)
    {
        highScoreText.text = "High Score: " + score.ToString();
    }

    public void UpdateMovesText(int moves)
    {
        moveText.text = " Moves \n" + moves.ToString();
    }

    public IEnumerator ActivateGameOverPanel()
    {
        yield return new WaitForSeconds(1.0f);
        gameOverPanel.SetActive(true);
    }
}
