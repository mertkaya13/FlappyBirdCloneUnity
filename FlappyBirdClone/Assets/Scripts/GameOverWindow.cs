using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;

    private Button restartButton;
    private Button mainMenuButton;
    private EventTrigger mouseOver;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("HighscoreCheckText").GetComponent<Text>();

        restartButton = transform.Find("RetryButton").GetComponent<Button>();
        mainMenuButton = transform.Find("MainMenuButton").GetComponent<Button>();

        restartButton.onClick.AddListener(restartGame);
        mainMenuButton.onClick.AddListener(mainMenuNav);
    }

    private void mainMenuNav() 
    {
        Loader.Load(Loader.Scene.MainMenu);
        HideThisWindow();
    }

    private void restartGame()
    {
        Loader.Load(Loader.Scene.GameScene);
        HideThisWindow();
    }

    private void Start()
    {
        HideThisWindow();
        Bird.getInstance().OnDied += Bird_OnDied;
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        showThisWindow();
        scoreText.text = Level.getInstance().returnScore().ToString();
        if(Level.getInstance().returnScore() >= Score.getHighScore())
        {
            //New High
            highscoreText.text = "NEW HIGHSCORE!!";
        }
        else
        {
            highscoreText.text = "HIGHSCORE : " + Score.getHighScore();
        }
    }

    private void HideThisWindow()
    {
        gameObject.SetActive(false);
    }

    private void showThisWindow()
    {
        gameObject.SetActive(true);
    }
}
