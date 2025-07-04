using UnityEngine;
using UnityEngine.UI;

public class Level1IntroUIManager : MonoBehaviour
{
    public static bool IsIntroOpen { get; private set; }

    [Header("UI refs")]
    [SerializeField] GameObject introPanel;
    [SerializeField] Button startButton;

    private void Awake()
    {
        IsIntroOpen = true;
        introPanel.SetActive(true);

        UnlockCursor();                 
        startButton.onClick.AddListener(CloseIntro);
    }

    private void Update()
    {
        if (IsIntroOpen &&
           (Cursor.lockState != CursorLockMode.None || !Cursor.visible))
            UnlockCursor();
    }

    private void CloseIntro()
    {
        introPanel.SetActive(false);
        IsIntroOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindObjectOfType<PauseMenuManager>()?.ForceUpdateCursor();
    }

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}


