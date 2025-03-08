using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateMusicButton : MonoBehaviour
{
    private ContentLoading ContentLoading;
    private float currentBPM = 120f; // Default BPM
    [SerializeField] TMP_InputField bpmInputField;
    [SerializeField] Button finishButton;

    private void Start()
    {
        // Add listener for BPM input field
        if (bpmInputField != null)
        {
            bpmInputField.onEndEdit.AddListener(OnBPMChanged);
        }
        if (bpmInputField != null)
        {
            finishButton.onClick.AddListener(() => FinishMusic());
        }
    }
    void OnBPMChanged(string newBPM)
    {
        if (float.TryParse(newBPM, out float bpmValue))
        {
            currentBPM = Mathf.Clamp(bpmValue, 60f, 160f); // Prevent unreasonable values
            Debug.Log("BPM updated to: " + currentBPM);
        }
        else
        {
            Debug.LogWarning("Invalid BPM input. Please enter a valid number.");
        }

    }
    public void FinishMusic()
    {
        ContentLoading.LoadAudio(currentBPM);
    }
    public void AssigngContentLoading(ContentLoading ContentLoad)
    {
        ContentLoading = ContentLoad;
    }
}
