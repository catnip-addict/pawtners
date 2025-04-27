using UnityEngine;
[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/Achievement")]
[System.Serializable]
public class Achievement : ScriptableObject
{
    public int id;
    public string title;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    [HideInInspector]
    public bool isUnlocked;

    public Achievement(int id, string title, string description, Sprite icon)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.icon = icon;
        this.isUnlocked = false;
    }
}