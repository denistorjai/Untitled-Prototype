using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    
}
