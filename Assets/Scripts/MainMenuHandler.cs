using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{

    public GameObject MenuCanvas;
    public GameObject SettingsCanvas;
    
    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public string CurrentMenu = "Main";

    public void OpenSettings()
    {
        CurrentMenu = "Settings";
        MenuCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        CurrentMenu = "MainMenu";
        MenuCanvas.SetActive(true);
        SettingsCanvas.SetActive(false);
    }
    
}
