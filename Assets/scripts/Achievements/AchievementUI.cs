using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;
    private Achievement _achievement;

    public void SetupAchievement(Achievement achievement, bool isUnlocked)
    {
        _achievement = achievement;

        if (titleText != null) titleText.text = achievement.title;
        if (descriptionText != null) descriptionText.text = achievement.description;

        if (iconImage != null && achievement.icon != null)
        {
            Debug.Log($"Setting icon for achievement: {achievement.title}");
            iconImage.sprite = achievement.icon;
        }

        UpdateUnlockState(isUnlocked);
    }

    public void UpdateUnlockState(bool isUnlocked)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isUnlocked ? unlockedColor : lockedColor;
        }

        if (iconImage != null)
        {
            iconImage.color = isUnlocked ? unlockedColor : lockedColor;
        }
    }

    public Achievement GetAchievement()
    {
        return _achievement;
    }
}