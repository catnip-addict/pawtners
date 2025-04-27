using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public CutsceneManage cutsceneManager;
    public int playerID; // 1 dla gracza 1, 2 dla gracza 2

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            cutsceneManager.PlayerEntered(playerID);
            gameObject.SetActive(false); // dezaktywuj trigger po wejœciu
        }
    }
}
