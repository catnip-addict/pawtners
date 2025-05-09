using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class KanarekManager : MonoBehaviour
{
    public static KanarekManager instance;

    public Animator DialogueAnimator;
    public TextMeshProUGUI TutorialText;
    public string[] Sentences;
    private int currentIndex = 0;
    public float DialogueSpeed;

    private Coroutine activeCoroutine;
    private bool isDisplayingText = false;
    private Queue<int> sentenceQueue = new Queue<int>();//to zrobilo AI nwm jak dzia³a ale zarz¹dza kolejk¹

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextSentence()
    {
        if (currentIndex < Sentences.Length)
        {
            if (isDisplayingText)
            {
                sentenceQueue.Enqueue(currentIndex);
                currentIndex++;
            }
            else
            {
                DialogueAnimator.SetTrigger("Enter");
                DisplaySentence(currentIndex);
                currentIndex++;
            }
        }
    }

    private void DisplaySentence(int index)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        TutorialText.text = "";
        isDisplayingText = true;
        activeCoroutine = StartCoroutine(WriteSentence(index));
    }

    IEnumerator WriteSentence(int index)
    {
        foreach (char Character in Sentences[index].ToCharArray())
        {
            TutorialText.text += Character;
            yield return new WaitForSeconds(DialogueSpeed);
        }

        activeCoroutine = null;
        isDisplayingText = false;

        if (sentenceQueue.Count > 0)
        {
            int nextIndex = sentenceQueue.Dequeue();
            yield return new WaitForSeconds(0.8f);
            DisplaySentence(nextIndex);
        }
        else
        {
            DialogueAnimator.SetTrigger("Exit");
        }
    }
}
