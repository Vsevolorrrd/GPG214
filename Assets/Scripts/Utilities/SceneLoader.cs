using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject obj;

    private static SceneLoader _instance;

    #region Singleton
    public static SceneLoader Instance
    {
        get
        {
            // Check if the instance is already created
            if (_instance == null)
            {
                // Try to find an existing SceneLoader in the scene
                _instance = FindAnyObjectByType<SceneLoader>();

                // If no SceneLoader exists, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneLoader");
                    _instance = singletonObject.AddComponent<SceneLoader>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        // If the instance is already set, destroy this duplicate object
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;  // Assign this object as the instance
        }
    }
    #endregion

    private void Start()
    {
        if (obj)
        obj.SetActive(true);

        if (SceneManager.GetActiveScene().name != "MainMenu")
        BEAT_Manager.Instance.StartTheMusic();
    }
    public void LoadScene(string name, bool loadPlayerPos = false)
    {
        StartCoroutine(TransitionToScene(name, loadPlayerPos));
    }
    IEnumerator TransitionToScene(string name, bool loadPlayerPos)
    {
        if (anim)
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            yield return null;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(name);

        if (anim)
        anim.SetTrigger("End");
        if (loadPlayerPos)
        SetPlayerPosition();

        yield return new WaitForSeconds(1);

        BEAT_Manager.Instance.StartTheMusic();

    }
    private async void SetPlayerPosition()
    {
        await Task.Delay(100);

        Seva.HeartsTest.Instance.SetHearts(Seva.SaveSystem.Instance.hearts);// It also doesn't work without delays ):
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = Seva.SaveSystem.Instance.playerPos;
            player.GetComponent<CharacterController>().enabled = true;


            Debug.Log(player.name);
            Debug.Log("Player moved to saved position: " + Seva.SaveSystem.Instance.playerPos);
        }
        else
        {
            Debug.LogWarning("Player not found after scene load!");
        }
    }
}
