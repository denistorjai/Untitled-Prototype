using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;
    
    // Scenes
    const string GameScene = "Game";
    const string Menu = "Menu";
    const string Upgrade = "UpgradeScene";
    
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void EndGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Menu);
    }
    
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameScene);
        if (PlayerManager.Instance)
        {
            PlayerManager.Instance.StartGame();
        }
    }
    
    public void NextRound()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Upgrade);
        PlayerManager.Instance.SetUpgrades();
    }
    
}
