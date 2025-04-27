using UnityEngine;

public class AchievementManagerUnlocker : MonoBehaviour
{
    public int achievementIndex = 0;
    public void UnlockAchievement()
    {
        AchievementManager.Instance.UnlockAchievement(achievementIndex);
    }
}
