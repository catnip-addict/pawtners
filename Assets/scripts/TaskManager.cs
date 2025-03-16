using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public string[] aktyGry = { "Tutorial", "Akt1", "Akt2", "Akt3", "Akt4", "Akt5" };
    public string[] zadania;
    public string[] wykonaneZadania;
    public TMP_Text MainTaskUI;
    public TMP_Text taskDone1;
    public TMP_Text taskDone2;
    private bool playTutorial = true;
    int numerZadania = 0;

    public Player player1;
    public Player player2;

  
    public Canvas comicCanvas;  
    public Image[] comicParts;  
    private bool isComicShowing = false;

    private void Start()
    {
        UpdateZadania();
        comicCanvas.enabled = false;


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

        if (numerZadania == 6 && !isComicShowing)
        {
            StartCoroutine(ShowComic());
            isComicShowing = true;
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
        comicCanvas.enabled = true; 

        yield return new WaitForSeconds(1);
        comicParts[0].enabled = true;

        yield return new WaitForSeconds(2);
        comicParts[1].enabled = true;


        yield return new WaitForSeconds(2);
        comicParts[2].enabled = true;


        yield return new WaitForSeconds(2);
        comicParts[3].enabled = true;


    }
}
