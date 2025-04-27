using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AchievementsPanel : MonoBehaviour
{
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text progressText;

    [SerializeField] private GameObject panel;

    private List<AchievementUI> achievementUIItems = new List<AchievementUI>();

    private void Start()
    {
        if (AchievementManager.Instance != null)
        {
            RefreshAchievements();
        }
    }
    public void RefreshAchievements()
    {

        if (AchievementManager.Instance != null)
        {
            Dictionary<Achievement, bool> achievements = AchievementManager.Instance.GetAllAchievements();
            if (achievementUIItems.Count > 0)
            {
                int index = 0;
                foreach (var achievementPair in achievements)
                {
                    if (index < achievementUIItems.Count)
                    {
                        achievementUIItems[index].SetupAchievement(achievementPair.Key, achievementPair.Value);
                        index++;
                    }
                }
            }
            else
            {
                CreateAchievementItems(achievements);
            }
            UpdateProgressText(achievements);
        }
    }

    private void CreateAchievementItems(Dictionary<Achievement, bool> achievements)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        achievementUIItems.Clear();

        foreach (var achievementPair in achievements)
        {
            GameObject achievementObj = Instantiate(achievementPrefab, contentParent);
            achievementObj.SetActive(true);
            AchievementUI achievementUI = achievementObj.GetComponent<AchievementUI>();

            if (achievementUI != null)
            {
                achievementUI.SetupAchievement(achievementPair.Key, achievementPair.Value);
                achievementUIItems.Add(achievementUI);
            }
        }
    }

    private void UpdateProgressText(Dictionary<Achievement, bool> achievements)
    {
        if (progressText != null)
        {
            int unlockedCount = 0;
            foreach (var achievementPair in achievements)
            {
                if (achievementPair.Value)
                {
                    unlockedCount++;
                }
            }

            progressText.text = $"Postęp: {unlockedCount} / {achievements.Count}";
        }
    }
}