using Unity.Netcode;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private static PauseMenuManager staticReference;
    private bool isPaused = false;

    [Header("Holders")]
    [SerializeField] private GameObject pauseMenuHolder;   // menú de pausa completo
    [SerializeField] private GameObject settingsPanel;     // sub-menú “Settings” (hijo del menú de pausa)

    /*  Acceso global al estado de pausa  */
    public static bool IsPaused => staticReference != null && staticReference.isPaused;

    /* ─────────────────────────────── Ciclo de vida ─────────────────────────────── */
    private void Awake()
    {
        if (staticReference != null)
            Destroy(staticReference.gameObject);

        staticReference = this;

        pauseMenuHolder.SetActive(isPaused);     // al arrancar, menú cerrado
        if (settingsPanel != null)               // asegurar que Settings también arranca cerrado
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            /*  Alternar pausa  */
            isPaused = !isPaused;
            pauseMenuHolder.SetActive(isPaused);

            /*  Si cerramos la pausa, cerramos también Settings (requisito nuevo)  */
            if (!isPaused && settingsPanel != null)
                settingsPanel.SetActive(false);

            UpdateCursorState();                 // cursor coherente para cualquier combinación
        }
    }

    /* ─────────────────────────────── Cursor / UI ─────────────────────────────── */
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

    /*  Llamada externa (por ejemplo, desde otros scripts/UI)  */
    public void ForceUpdateCursor() => UpdateCursorState();

    /* ─────────────────────────────── Botones UI ─────────────────────────────── */
    public void DisconnectFromServer() => NetworkManager.Singleton.Shutdown();
    public void Exit() => Application.Quit();
}





