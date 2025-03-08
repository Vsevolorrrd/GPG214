using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public string saveFilePath;
    public string sceneToLoad;
    public Vector3 playerPos;
    public int hearts;

    private static SaveSystem _instance;

    #region Singleton
    public static SaveSystem Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("SaveSystem already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    public string folderPath;

    void Start()
    {
        folderPath = Path.Combine(Application.persistentDataPath, "GameData");
        saveFilePath = Path.Combine(folderPath, "saveData.txt");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created Folder: " + folderPath);
        }

        if (!File.Exists(saveFilePath))
        {
            File.WriteAllText(saveFilePath, "");
            Debug.Log("Created File: " + saveFilePath);
        }

        LoadGameData();
    }

    public void SaveGameData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        playerPos = player.transform.position;
        else
        Debug.LogWarning("Player not found");

        sceneToLoad = SceneManager.GetActiveScene().name;

        string saveData = $"{playerPos.x},{playerPos.y},{playerPos.z}\n{hearts}\n{sceneToLoad}";
        File.WriteAllText(saveFilePath, saveData);

        Debug.Log("Game Data Saved");
    }

    public void LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string[] lines = File.ReadAllLines(saveFilePath);
            if (lines.Length >= 3)
            {
                string[] posValues = lines[0].Split(',');
                if (posValues.Length == 3)
                {
                    float.TryParse(posValues[0], out playerPos.x);
                    float.TryParse(posValues[1], out playerPos.y);
                    float.TryParse(posValues[2], out playerPos.z);
                }
                else
                {
                    Debug.LogWarning("Invalid position data, setting to default.");
                    playerPos = Vector3.zero;
                }

                int.TryParse(lines[1], out hearts);
                sceneToLoad = lines[2];

                Debug.Log("Game Data Loaded");
            }
        }
        else
        {
            playerPos = Vector3.zero;
            hearts = 0;
            sceneToLoad = "Tutorial";
            Debug.Log("No Save File Found, Creating New Data");
        }
    }
}