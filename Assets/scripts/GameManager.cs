using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string currentSceneName;
    public Player player1;
    public Player player2;
    public int mouseCont = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void FindPlayers()
    {
        player1 = GameObject.Find("Player1").GetComponent<Player>();
        player2 = GameObject.Find("Player2").GetComponent<Player>();
        Debug.Log("Player1: " + player1.name + " Player2: " + player2.name);
    }

    public void AddToMouse()
    {
        mouseCont++;
        if (mouseCont >= 20)
        {
            AchievementManager.Instance.UnlockAchievement(13);
        }
    }
}
