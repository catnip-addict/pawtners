using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        KanarekManager.instance.NextSentence();
        Destroy(gameObject);
    }
}
