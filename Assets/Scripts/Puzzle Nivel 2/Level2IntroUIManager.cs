using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkObject))]
public class Level2IntroUIManager : NetworkBehaviour
{
    public static bool IsIntroOpen { get; private set; }

    [SerializeField] GameObject introPanel;
    [SerializeField] Button startButton;

    void Awake()
    {
        IsIntroOpen = true;
        introPanel.SetActive(true);
        UnlockCursor();

        startButton.onClick.AddListener(OnStartClicked);
    }

    void Update()
    {
        if (IsIntroOpen &&
           (Cursor.lockState != CursorLockMode.None || !Cursor.visible))
            UnlockCursor();
    }

    private void OnStartClicked()
    {
        introPanel.SetActive(false);
        IsIntroOpen = false;
        LockCursor();

        Debug.Log("[CLIENT " + OwnerClientId + "] Pulsó Comenzar.");


        RequestStartTimerServerRpc();

        FindObjectOfType<PauseMenuManager>()?.ForceUpdateCursor();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestStartTimerServerRpc()
    {
        if (Level2Timer.Instance != null)
            Level2Timer.Instance.StartCountdown();
    }

    static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }
    static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }
}





