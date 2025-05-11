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
    private Queue<int> sentenceQueue = new Queue<int>();//to zrobilo AI nwm jak dzia�a ale zarz�dza kolejk�

    [Header("Sprite Settings")]
    [SerializeField] private TMP_SpriteAsset spriteAsset; // Reference to your sprite asset

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

    private void Start()
    {
        // Apply sprite asset to the TextMeshProUGUI if provided
        if (spriteAsset != null && TutorialText != null)
        {
            TutorialText.spriteAsset = spriteAsset;
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
        string sentence = Sentences[index];
        string currentText = "";
        bool inTag = false;
        string tagBuffer = "";

        for (int i = 0; i < sentence.Length; i++)
        {
            char character = sentence[i];

            // Check for tag start
            if (character == '<')
            {
                inTag = true;
                tagBuffer = "<";
                continue;
            }
            // Check for tag end
            else if (inTag && character == '>')
            {
                inTag = false;
                tagBuffer += '>';
                currentText += tagBuffer; // Add the entire tag at once
                TutorialText.text = currentText;
                continue;
            }
            // Build tag
            else if (inTag)
            {
                tagBuffer += character;
                continue;
            }

            // Normal character display
            currentText += character;
            TutorialText.text = currentText;

            // Only wait for normal characters
            yield return new WaitForSeconds(DialogueSpeed);
        }

        activeCoroutine = null;

        if (sentenceQueue.Count > 0)
        {
            int nextIndex = sentenceQueue.Dequeue();
            yield return new WaitForSeconds(0.8f);
            DisplaySentence(nextIndex);
        }
        else
        {
            yield return new WaitForSeconds(5f);
            if (sentenceQueue.Count > 0)
            {
                int nextIndex = sentenceQueue.Dequeue();
                yield return new WaitForSeconds(0.8f);
                DisplaySentence(nextIndex);
            }
            else
            {
                isDisplayingText = false;
                DialogueAnimator.SetTrigger("Exit");
            }
        }
    }
}
