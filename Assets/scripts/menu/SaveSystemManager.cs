using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystemManager : MonoBehaviour
{
    public static SaveSystemManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool SaveFileExists()
    {
        string path = Application.persistentDataPath + "/gamesave.save";
        return File.Exists(path);
    }

    public void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/gamesave.save";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void SaveGame(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gamesave.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/gamesave.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Nie znaleziono pliku zapisu!");
            return null;
        }
    }
}

[System.Serializable]
public class GameData
{
    public int playerLevel;
    public float playTime;
    public string playerName;

    public GameData(int level, float time, string name)
    {
        playerLevel = level;
        playTime = time;
        playerName = name;
    }
}
