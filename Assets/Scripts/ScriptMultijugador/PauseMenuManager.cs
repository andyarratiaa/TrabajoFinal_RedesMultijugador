using Unity.Netcode;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private static PauseMenuManager staticReference;
    private bool isPaused = false;

    [SerializeField] private GameObject pauseMenuHolder;

    /* Acceso global al estado de pausa */
    public static bool IsPaused => staticReference != null && staticReference.isPaused;

    private void Awake()
    {
        if (staticReference != null)
            Destroy(staticReference.gameObject);

        staticReference = this;
        pauseMenuHolder.SetActive(isPaused);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenuHolder.SetActive(isPaused);

            UpdateCursorState();          // ← ¡usa la lógica centralizada!
        }
    }

    private void UpdateCursorState()
    {
        bool loseOpen = Level2UIManager.IsLosePanelOpen;
        bool introOpen = Level1IntroUIManager.IsIntroOpen;   // ← NUEVO
        bool intro2Open = Level2IntroUIManager.IsIntroOpen;

        bool show = isPaused || loseOpen || introOpen || intro2Open;

        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }

    /*  Coloca el método dentro de la clase PauseMenuManager,
    justo debajo de UpdateCursorState() o donde prefieras. */
    public void ForceUpdateCursor()
    {
        UpdateCursorState();
    }



    /* ── Llamadas desde los botones del menú ── */
    public void DisconnectFromServer() => NetworkManager.Singleton.Shutdown();
    public void Exit() => Application.Quit();
}




