using System.Threading.Tasks.Sources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    //Publiczba lista tasków, po ukoñczeniu wyœwietla jako ukoñczone zadanie

    public string[] aktyGry = {"Tutorial", "Akt1 - Ucieczka od Hycla", "Akt2", "Akt3", "Akt4", "Akt5"};
    //public int[] aktualnyRozdzial = { 1, 2, 3 };
    public string[] zadania; //Lista wszystkich zadañ
    public string[] wykonaneZadania;
    public TMP_Text MainTaskUI;
    public TMP_Text taskDone1;
    public TMP_Text taskDone2;
    private bool playTutorial = true;
    public int numerZadania = 0;
    public bool taskCompleted = false;

    public Player player1;
    public Player player2;

    //Œmieciowe zmienne do wykonywania tutoriali
    private bool czy1, czy2, czy3, czy4;


    private void Start()
    {
        UpdateZadania();
        czy1 = czy2 = czy3 = czy4 = false;
    }

    private void Update()
    {
        if(numerZadania == 0)
        {
            taskCompleted = false;
            float x = player1.moveInput.x;
            float y = player1.moveInput.y;
            //Prosze zmien to
            
            if (x == 1) { czy1 = true; }
            if (x == -1) { czy2 = true; }
            if (y == 1) { czy3 = true; }
            if (y == -1) { czy4 = true; }
            if (czy1 && czy2 && czy3 && czy4) { taskCompleted = true; }

            if (taskCompleted)
            {
                numerZadania++;
                UpdateZadania();
                taskCompleted = false;
            }
        }
        else if(numerZadania == 1)
        {
            //Cale zadanie wykonuje skrypt TableScript
            if (taskCompleted)
            {
                numerZadania++;
                UpdateZadania();
                taskCompleted=false;
            }
        }
        else if (numerZadania == 2)
        {
            Debug.Log("Drapanie do zrobienia!");
            taskCompleted = true ;

            if (taskCompleted)
            {
                numerZadania++;
                UpdateZadania();
                taskCompleted = false;
            }
        }
        else if (numerZadania == 3)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.Y))
            {
                Debug.Log("Mia³³³³");
                taskCompleted = true ;
            }

            if (taskCompleted)
            {
                numerZadania++;
                UpdateZadania();
                taskCompleted = false;
            }
        }
        else if (numerZadania == 4)
        {
            //Ca³e zadanie wykonuje OknoScript
            if (taskCompleted)
            {
                numerZadania++;
                UpdateZadania();
                taskCompleted = false;
            }
        }
    }

    void UpdateZadania()
    {
        if (playTutorial)//Po ukoñczeniu bêdzie mo¿na u¿yæ totalnie inaczej do póŸniejszych aktów
        {
            MainTaskUI.text = zadania[numerZadania];
            if (numerZadania == 1)
            {
                taskDone1.text = zadania[0];
            }
            if (numerZadania >= 2)
            {
                taskDone1.text = zadania[numerZadania - 1];
                taskDone2.text = zadania[numerZadania - 2];
            }
        } 


    }

}
