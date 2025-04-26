using System;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static string saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}

// [Serializable]
// public class GameData
// {
//     public int playerLevel;
//     public float playerHealth;
//     public float playerScore;
//     // Add other game data fields as needed
// }