using System.Collections;
using UnityEngine;

public class CutsceneManage : MonoBehaviour
{
    public GameObject cutsceneObject;
    public Animator cutsceneAnim;
    public Camera playerCam1;
    public Camera playerCam2;
    public Camera cutsceneCam;
    public Player playerScript;
    public string cutsceneTriggerName;
    public float animationTime;

    private bool player1Ready = false;
    private bool player2Ready = false;


    public void PlayerEntered(int playerID)
    {

        if (playerID == 1)
            player1Ready = true;
        else if (playerID == 2)
            player2Ready = true;

        CheckIfStartCutscene();
    }

    public void CheckIfStartCutscene()
    {
        StartCoroutine(StartCutscene());
    }

    private IEnumerator StartCutscene()
    {

        yield return new WaitForSeconds(0.5f);

        playerScript.enabled = false;
        cutsceneObject.SetActive(true);
        cutsceneCam.gameObject.SetActive(true);
        playerCam1.gameObject.SetActive(false);
        playerCam2.gameObject.SetActive(false);

        cutsceneAnim.SetTrigger(cutsceneTriggerName);

        yield return new WaitForSeconds(animationTime);

        playerScript.enabled = true;
        playerCam1.gameObject.SetActive(true);
        playerCam2.gameObject.SetActive(true);
        cutsceneCam.gameObject.SetActive(false);
        cutsceneObject.SetActive(false);
    }
}
