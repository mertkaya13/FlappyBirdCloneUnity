using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("Play Button").GetComponent<Button>().onClick.AddListener(loadGame);
        transform.Find("Quit Button").GetComponent<Button>().onClick.AddListener(quitGame);
    }

    public void loadGame()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
