using Unity.Netcode;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private static PauseMenuManager staticReference;
    private bool isPaused = false;

    [Header("Holders")]
    [SerializeField] private GameObject pauseMenuHolder; 
    [SerializeField] private GameObject settingsPanel;

    public static bool IsPaused => staticReference != null && staticReference.isPaused;


    private void Awake()
    {
        if (staticReference != null)
            Destroy(staticReference.gameObject);

        staticReference = this;

        pauseMenuHolder.SetActive(isPaused);    
        if (settingsPanel != null)               
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenuHolder.SetActive(isPaused);

            if (!isPaused && settingsPanel != null)
                settingsPanel.SetActive(false);

            UpdateCursorState();                 
        }
    }

    private void UpdateCursorState()
    {
        bool settingsOpen = settingsPanel != null && settingsPanel.activeSelf;
        bool loseOpen = Level2UIManager.IsLosePanelOpen;
        bool introOpen = Level1IntroUIManager.IsIntroOpen;
        bool intro2Open = Level2IntroUIManager.IsIntroOpen;

        bool show = isPaused || settingsOpen || loseOpen || introOpen || intro2Open;

        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }

    public void ForceUpdateCursor() => UpdateCursorState();

    public void DisconnectFromServer() => NetworkManager.Singleton.Shutdown();
    public void Exit() => Application.Quit();
}





