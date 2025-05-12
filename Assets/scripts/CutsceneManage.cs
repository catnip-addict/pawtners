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
        StartCoroutine(TransitionManager.Instance.TransitionToScene(3));
        // playerScript.enabled = true;
        // playerCam1.gameObject.SetActive(true);
        // playerCam2.gameObject.SetActive(true);
        // cutsceneCam.gameObject.SetActive(false);
        // cutsceneObject.SetActive(false);
    }
}
