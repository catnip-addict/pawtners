using UnityEngine;
using TMPro;
using System.Collections;

public class KanarekManager : MonoBehaviour
{
    public static KanarekManager instance;

    public TextMeshProUGUI TutorialText;
    public string[] Sentences;
    private int Index = 0;
    public float DialogueSpeed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    public void NextSentence()
    {
        if(Index <= Sentences.Length -1)
        {
            TutorialText.text = "";
            StartCoroutine(WriteSentence());
        }
    }

    IEnumerator WriteSentence()
    {
        foreach(char Character in Sentences[Index].ToCharArray())
        {
            TutorialText.text += Character;
            yield return new WaitForSeconds(DialogueSpeed);
        }
        Index++;
    }
}
