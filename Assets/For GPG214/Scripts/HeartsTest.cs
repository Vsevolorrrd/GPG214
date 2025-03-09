using TMPro;
using UnityEngine;

namespace Seva
{
    public class HeartsTest : MonoBehaviour
    {
        public int Hearts = 0;
        public TextMeshProUGUI text;

        private static HeartsTest _instance;

        #region Singleton
        public static HeartsTest Instance => _instance;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("HeartsTest already exists, destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        #endregion
        public void AddHearts(int hearts)
        {
            Hearts += hearts;
            text.text = "Hearts: " + Hearts.ToString();
        }
        public void SetHearts(int hearts)
        {
            Hearts = hearts;
            text.text = "Hearts: " + Hearts.ToString();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                AddHearts(1);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                AddHearts(-1);
            }
        }
    }
}