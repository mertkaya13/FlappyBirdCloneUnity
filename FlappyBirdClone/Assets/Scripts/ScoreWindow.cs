using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text highScoreText;
    private Text scoreText;
    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("highScoreText").GetComponent<Text>();
    }

    private void Start()
    {
        highScoreText.text = "HIGHSCORE:" + Score.getHighScore().ToString();
    }

    private void Update()
    {
        scoreText.text = "Score : "+ Level.getInstance().returnScore().ToString();
    }
}
