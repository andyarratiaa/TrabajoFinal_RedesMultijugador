//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class Level2UIManager : MonoBehaviour
//{
//    public static Level2UIManager Instance;

//    [Header("UI refs")]
//    [SerializeField] TMP_Text timerText;
//    [SerializeField] GameObject losePanel;
//    [SerializeField] Button retryButton;

//    private void Awake()
//    {
//        Instance = this;

//        // Cada vez que la escena se carga, parte de cero
//        IsLosePanelOpen = false;          // ← añade esta línea
//        losePanel.SetActive(false);

//        retryButton.onClick.AddListener(Retry);
//    }

//    void Update()
//    {
//        /* Si el panel de derrota está activo pero
//           el cursor se ha quedado bloqueado por otro script,
//           lo volvemos a liberar. */
//        if (IsLosePanelOpen &&
//           (Cursor.lockState != CursorLockMode.None || !Cursor.visible))
//        {
//            Cursor.lockState = CursorLockMode.None;
//            Cursor.visible = true;
//        }
//    }


//    /* Actualiza cronómetro */
//    public void UpdateTimer(float seconds)
//    {
//        int m = Mathf.FloorToInt(seconds / 60);
//        int s = Mathf.FloorToInt(seconds % 60);
//        timerText.text = $"{m:00}:{s:00}";
//    }

//    public static bool IsLosePanelOpen { get; private set; }   // ← NUEVO

//    public void ShowLosePanel()
//    {
//        losePanel.SetActive(true);
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;

//        IsLosePanelOpen = true;
//    }


//    private void Retry()
//    {
//        // Cierra el panel y desbloquea el input antes de pedir el restart
//        losePanel.SetActive(false);
//        IsLosePanelOpen = false;
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;

//        FindObjectOfType<Level2Timer>()
//            .RestartLevelServerRpc();
//    }
//}

// Assets/Scripts/Level2/Level2UIManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level2UIManager : MonoBehaviour
{
    public static Level2UIManager Instance;

    [Header("UI refs")]
    [SerializeField] private TMP_Text timerText;    // Texto 02:00
    [SerializeField] private GameObject losePanel;    // Panel de derrota
    [SerializeField] private Button retryButton;  // Botón “Reintentar”

    public static bool IsLosePanelOpen { get; private set; }

    /* ────────────── life-cycle ────────────── */
    private void Awake()
    {
        Instance = this;

        IsLosePanelOpen = false;
        losePanel.SetActive(false);

        retryButton.onClick.AddListener(Retry);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /* ────────────── API pública ────────────── */
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

    /* ────────────── Botón “Reintentar” ────────────── */
    private void Retry()
    {
        losePanel.SetActive(false);
        IsLosePanelOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Level2Timer.Instance?.RestartLevelServerRpc();
    }

    /* ────────────── Cambio de escena ────────────── */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si NO estamos en Level2 (por ejemplo, “Win Scene”), ocultar HUD
        if (scene.name != "Level2")
            gameObject.SetActive(false);   // (o Destroy(gameObject); si prefieres)
    }
}
