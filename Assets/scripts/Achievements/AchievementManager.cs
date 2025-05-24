using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [SerializeField] private List<Achievement> availableAchievements = new List<Achievement>();
    [SerializeField] private GameObject achievementNotification;
    [SerializeField] private float notificationDuration = 3f;

    private Dictionary<Achievement, bool> unlockedAchievements = new Dictionary<Achievement, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAchievementStates();
    }

    private void InitializeAchievements()
    {
        foreach (Achievement achievement in availableAchievements)
        {
            unlockedAchievements.Add(achievement, false);
        }
    }
    private void LoadAchievementStates()
    {
        if (SaveSystemManager.instance != null && SaveSystemManager.instance.SaveFileExists())
        {
            GameData data = SaveSystemManager.instance.LoadGame();
            if (data != null && data.achievements != null && data.achievements.Count > 0)
            {
                Dictionary<int, bool> savedStates = new Dictionary<int, bool>();
                for (int i = 0; i < data.achievements.Count; i++)
                {
                    savedStates[i] = data.achievements[i];
                }

                foreach (Achievement achievement in availableAchievements)
                {
                    if (savedStates.ContainsKey(achievement.id))
                    {
                        unlockedAchievements[achievement] = savedStates[achievement.id];
                    }
                }

                Debug.Log($"Załadowano {unlockedAchievements.Count(kvp => kvp.Value)} odblokowanych osiągnięć");

                //Do zresetowania osiągnięć
                // foreach (Achievement achievement in availableAchievements)
                // {
                //     unlockedAchievements[achievement] = false;
                // }

                // // Zapisanie zresetowanych osiągnięć
                // SaveAchievementStates();

                // Debug.Log("Wszystkie osiągnięcia zostały zresetowane");
            }
        }
    }
    public void ResetAchievements()
    {
        foreach (Achievement achievement in availableAchievements)
        {
            unlockedAchievements[achievement] = false;
        }

        SaveAchievementStates();
        LoadAchievementStates();
        Debug.Log("Wszystkie osiągnięcia zostały zresetowane");
    }
    public void SaveAchievementStates()
    {
        if (SaveSystemManager.instance != null)
        {
            GameData data = null;

            if (SaveSystemManager.instance.SaveFileExists())
            {
                data = SaveSystemManager.instance.LoadGame();
            }
            else
            {
                data = new GameData(0);
            }

            if (data != null)
            {
                int maxId = 0;
                foreach (var achievement in availableAchievements)
                {
                    maxId = Mathf.Max(maxId, achievement.id);
                }

                while (data.achievements.Count <= maxId)
                {
                    data.achievements.Add(false);
                }

                foreach (var achievementPair in unlockedAchievements)
                {
                    int id = achievementPair.Key.id;
                    if (id >= 0 && id < data.achievements.Count)
                    {
                        data.achievements[id] = achievementPair.Value;
                    }
                }

                SaveSystemManager.instance.SaveGame(data);
                Debug.Log("Zapisano osiągnięcia");
            }
        }
    }
    public void UnlockAchievement(int id)
    {
        Achievement achievement = availableAchievements.FirstOrDefault(a => a.id == id);
        if (achievement != null)
        {
            UnlockAchievement(achievement);
        }
    }

    public void UnlockAchievement(Achievement achievement)
    {
        if (achievement != null && unlockedAchievements.ContainsKey(achievement) && !unlockedAchievements[achievement])
        {
            unlockedAchievements[achievement] = true;
            ShowNotification(achievement);
            SaveAchievementStates();
            Debug.Log("Osiągnięcie odblokowane: " + achievement.title);
        }
    }

    public Dictionary<Achievement, bool> GetAllAchievements()
    {
        return unlockedAchievements;
    }

    public bool IsAchievementUnlocked(int id)
    {
        Achievement achievement = availableAchievements.FirstOrDefault(a => a.id == id);
        if (achievement != null && unlockedAchievements.ContainsKey(achievement))
        {
            return unlockedAchievements[achievement];
        }
        return false;
    }

    private void ShowNotification(Achievement achievement)
    {
        if (achievementNotification != null)
        {
            achievementNotification.SetActive(true);
            achievementNotification.GetComponent<AchievementUI>().SetupAchievement(achievement, achievement.isUnlocked);
            Invoke("HideNotification", notificationDuration);
        }
    }

    private void HideNotification()
    {
        if (achievementNotification != null)
        {
            achievementNotification.SetActive(false);
        }
    }
}