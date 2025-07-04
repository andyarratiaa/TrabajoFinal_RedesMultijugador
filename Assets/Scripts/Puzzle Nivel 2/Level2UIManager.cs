using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level2UIManager : MonoBehaviour
{
    public static Level2UIManager Instance;

    [Header("UI refs")]
    [SerializeField] private TMP_Text timerText; 
    [SerializeField] private GameObject losePanel;  
    [SerializeField] private Button retryButton;  

    public static bool IsLosePanelOpen { get; private set; }

    private void Awake()
    {
        Instance = this;

        IsLosePanelOpen = false;
        losePanel.SetActive(false);

        retryButton.onClick.AddListener(Retry);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public void UpdateTimer(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        timerText.text = $"{m:00}:{s:00}";
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        IsLosePanelOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Retry()
    {
        losePanel.SetActive(false);
        IsLosePanelOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Level2Timer.Instance?.RestartLevelServerRpc();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Level2")
            gameObject.SetActive(false); 
    }
}
