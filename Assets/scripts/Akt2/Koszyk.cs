using System.Collections.Generic;
using UnityEngine;

public class Koszyk : MonoBehaviour
{
    [SerializeField] private GameObject[] applePrefab;
    [SerializeField] private List<Apple> addedApples;
    [SerializeField] private KanarekManager kanarek;
    [SerializeField] private Animator jezykAnimator;
    [SerializeField] private Collider colliderRura;
    [SerializeField] private int appleCount = 0;
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Apple>(out var apple))
        {
            if (addedApples.Contains(apple))
            {
                return;
            }
            other.gameObject.SetActive(false);
            applePrefab[appleCount].SetActive(true);
            addedApples.Add(apple);
            appleCount++;
            if (appleCount >= 5)
            {
                Debug.Log("Zebrałem wszystkie jabłka");
                colliderRura.enabled = false;
                jezykAnimator.SetTrigger("idz");
                kanarek.PlaySentence(6);
            }
        }
    }
}
