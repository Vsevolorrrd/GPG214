using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Seva
{

}
public class ContentLoading : MonoBehaviour
{
    public string musicFolderPath;
    public string musicFolderName = "Music";
    private string currentMusicPath;
    [SerializeField] CreateMusicButton musicButton;
    [SerializeField] GameObject musicCreationUI;
    [SerializeField] Button buttonPrefab;

    [SerializeField] Image displayImageTest;
    public string imageName = "TestImage";

    private Dictionary<string, Button> musicButtons = new Dictionary<string, Button>();

    void Start()
    {
        StartCoroutine(LoadImage());
        musicButton.AssigngContentLoading(this);
        musicCreationUI.SetActive(false);

        musicFolderPath = Path.Combine(Application.streamingAssetsPath, musicFolderName);

        if (!Directory.Exists(musicFolderPath))
        {
            Directory.CreateDirectory(musicFolderPath);
            Debug.Log("Music folder not found, creating new folder");
        }
    }

    public void LoadAllAudio(Transform container)
    {
        if (!Directory.Exists(musicFolderPath))
        {
            Debug.LogError("Music folder not found: " + musicFolderPath);
            return;
        }

        string[] audioFiles = Directory.GetFiles(musicFolderPath, "*.mp3");

        // Remove buttons that no longer correspond to files
        List<string> existingFiles = new List<string>(musicButtons.Keys);
        foreach (string existingFile in existingFiles)
        {
            if (!System.Array.Exists(audioFiles, file => Path.GetFileNameWithoutExtension(file) == existingFile))
            {
                Destroy(musicButtons[existingFile].gameObject);
                musicButtons.Remove(existingFile);
            }
        }

        foreach (string file in audioFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (!musicButtons.ContainsKey(fileName))
            {
                Button newButton = Instantiate(buttonPrefab, container);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
                newButton.onClick.AddListener(() => OpenMusicCreationUI(file));
                musicButtons[fileName] = newButton;
            }
        }
    }
    void OpenMusicCreationUI(string filePath)
    {
        currentMusicPath = filePath;
        musicCreationUI.SetActive(true);
    }
    public void LoadAudio(float bpm)
    {
        musicCreationUI.SetActive(false);
        StartCoroutine(LoadAudioCoroutine(currentMusicPath, bpm));
    }

    IEnumerator LoadAudioCoroutine(string filePath, float bpm)
    {
        string url = "file://" + filePath; // Convert to URL format for UnityWebRequest
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

                Music newMusic = new Music
                {
                    songBPM = bpm, // Default BPM, can be adjusted later
                    Leadingtrack = audioClip,
                    track_2 = null,
                    track_3 = null,
                    transition = null
                };

                BEAT_Manager.Instance.SetNewMusic(newMusic);
            }
        }
    }

    IEnumerator LoadImage()
    {
        string texturePath = Path.Combine(Application.streamingAssetsPath, imageName);

        if (!File.Exists(texturePath))
        {
            Debug.LogError("Image file not found: " + texturePath);
            yield break;
        }

        string url = "file://" + texturePath; // Convert to URL format

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading image: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;

                // Convert Texture2D to Sprite
                Sprite newSprite = TextureToSprite(texture);
                // Apply to UI Image
                ApplySprite(newSprite);
            }
        }
    }
    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private void ApplySprite(Sprite sprite)
    {
        if (displayImageTest != null)
        {
            displayImageTest.sprite = sprite;
            Debug.Log("Sprite applied successfully!");
        }
        else
        {
            Debug.LogError("No UI Image assigned!");
        }
    }

}