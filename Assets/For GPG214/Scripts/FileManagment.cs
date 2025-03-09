using System.IO;
using UnityEngine;

namespace Seva
{
    public class FileManagment : MonoBehaviour
    {
        public string folderPath;
        public string filePath;

        void Start()
        {
            folderPath = Path.Combine(Application.persistentDataPath, "GameData");
            filePath = Path.Combine(folderPath, "saveData.txt");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Created Folder: " + folderPath);
            }

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                Debug.Log("Created File: " + filePath);
            }
        }
    }
}