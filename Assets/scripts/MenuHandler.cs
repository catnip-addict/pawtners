using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
