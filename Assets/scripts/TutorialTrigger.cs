using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        KanarekManager.instance.NextSentence();
        Destroy(gameObject);
    }
}
