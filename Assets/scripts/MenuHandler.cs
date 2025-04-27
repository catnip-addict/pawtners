using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject mainMenuFirst;
    public GameObject settingsMenu;
    public GameObject settingsMenuFirst;
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }
    public void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }
    public void ShowCreditsMenu()
    {
        Debug.Log("Wyświetlanie menu kredytów");
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
