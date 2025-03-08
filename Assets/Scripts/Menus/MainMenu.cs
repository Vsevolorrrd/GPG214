using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI; // Assign your pause menu UI in the Inspector
    [SerializeField] GameObject musicUI;
    [SerializeField] GameObject mainUI;
    [SerializeField] Transform container;
    private bool isPaused = false;
    public bool canOpenPauseMenu = false;
    public bool lockCursor = false;

    private ContentLoading contentLoading;
    public static Action<bool> OnPause;

    private void Start()
    {
        contentLoading = GetComponent<ContentLoading>();
    }
    void Update()
    {
        if (canOpenPauseMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ContinueGame()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene(SaveSystem.Instance.sceneToLoad, true);
    }
    public void StartGame()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void OpenMainMenu()
    {
        Time.timeScale =  1f;
        isPaused = false;
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
        SetPauseState(false);
    }

    public void TogglePause()
    {
        SetPauseState(!isPaused);
    }

    private void SetPauseState(bool state)
    {
        isPaused = state;
        if(pauseMenuUI)
        pauseMenuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : (lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
        Cursor.visible = isPaused || !lockCursor;

        OnPause?.Invoke(isPaused);
    }

    public void SaveGame()
    {
        SaveSystem.Instance.SaveGameData();
    }

    public void QuitGame()
    {
        SetPauseState(false);
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void OpenMusicUI()
    {
        if (musicUI == null || contentLoading == null) return;
        musicUI.SetActive(true);
        mainUI.SetActive(false);
        contentLoading.LoadAllAudio(container);
    }
    public void CloseMusicUI()
    {
        musicUI.SetActive(false);
        mainUI.SetActive(true);
    }
}