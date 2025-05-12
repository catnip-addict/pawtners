using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TaskManager : MonoBehaviour
{
    public string[] aktyGry = { "Tutorial", "Akt1", "Akt2", "Akt3", "Akt4", "Akt5" };
    public string[] zadania;
    public string[] wykonaneZadania;
    public TMP_Text MainTaskUI;
    public TMP_Text taskDone1;
    public TMP_Text taskDone2;
    public bool playTutorial = true;
    int numerZadania = 0;

    public Player player1;
    public Player player2;

    Mechaniki mechaniki1;
    Mechaniki mechaniki2;

    public GameObject comicCanvas;
    public Image[] comicParts;
    public TMP_Text comicText;
    private bool isComicShowing = false;
    private bool isComicDone = false;
    public int whichScene = 2;

    private void Start()
    {
        mechaniki1 = player1.GetMechaniki();
        mechaniki2 = player2.GetMechaniki();
        UpdateZadania();
        comicCanvas.SetActive(false);

        foreach (var comicPart in comicParts)
        {
            comicPart.enabled = false;
        }

    }

    private void Update()
    {
        if (numerZadania == 0)
        {
            if (player1.movement.magnitude > 0)
            {
                UpdateZadania(1);
            }
            else if (player2.movement.magnitude > 0)
            {
                UpdateZadania(1);
            }
        }
        if (numerZadania == 6 && !isComicShowing && whichScene == 0)
        {
            StartCoroutine(ShowComic2());
            isComicShowing = true;
        }
        if (numerZadania == 6 && !isComicShowing)
        {
            StartCoroutine(ShowComic());
            isComicShowing = true;
        }

        if (Input.anyKey && isComicDone)
        {
            // SceneManager.LoadScene(0);
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
            StartCoroutine(TransitionManager.Instance.TransitionToScene(whichScene));
        }

    }

    public void UpdateZadania(int index = 0)
    {
        if (numerZadania > index)
            return;

        numerZadania = index;
        if (playTutorial)
        {
            MainTaskUI.text = zadania[index];
            if (index == 1)
            {
                taskDone1.text = zadania[0];
            }
            if (index >= 2)
            {
                taskDone1.text = zadania[index - 1];
                taskDone2.text = zadania[index - 2];
            }
        }
    }


    IEnumerator ShowComic()
    {
        comicCanvas.SetActive(true);
        PauseMenu.Instance.isBusy = true;

        yield return new WaitForSeconds(1);
        comicParts[0].enabled = true;

        yield return new WaitForSeconds(2);
        comicParts[1].enabled = true;


        yield return new WaitForSeconds(2);
        comicParts[2].enabled = true;

        if (comicParts[3] != null)
        {
            yield return new WaitForSeconds(2);
            comicParts[3].enabled = true;
        }

        yield return new WaitForSeconds(2);
        isComicDone = true;
        comicText.text = "Naciśnij dowolny klawisz aby kontynuować...";
    }
    IEnumerator ShowComic2()
    {
        comicCanvas.SetActive(true);
        PauseMenu.Instance.isBusy = true;
        yield return new WaitForSeconds(1);
        comicParts[0].enabled = true;

        yield return new WaitForSeconds(2);
        comicParts[1].enabled = true;


        yield return new WaitForSeconds(2);
        comicParts[2].enabled = true;

        yield return new WaitForSeconds(2);
        isComicDone = true;
        comicText.text = "Naciśnij dowolny klawisz aby kontynuować...";
    }
    public void Tp(int id)
    {
        StartCoroutine(TransitionManager.Instance.TransitionToScene(id));
    }


    public void haveKey()
    {
        if (mechaniki1.haveObject || mechaniki2.haveObject)
        {
            Tp(3);
        }
    }
}
