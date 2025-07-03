using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Timer : NetworkBehaviour
{
    public static Level2Timer Instance { get; private set; }

    [SerializeField] float durationSeconds = 120f;

    readonly NetworkVariable<float> timeLeft = new();
    readonly NetworkVariable<bool> countdownStarted = new(false);

    bool defeatSent = false;

    public override void OnNetworkSpawn()
    {
        Instance = this;
        if (IsServer) timeLeft.Value = durationSeconds;

        Level2UIManager.Instance?.UpdateTimer(timeLeft.Value);

        timeLeft.OnValueChanged += (_, v) =>
            Level2UIManager.Instance?.UpdateTimer(v);
    }

    void Update()
    {
        if (!IsServer || defeatSent || !countdownStarted.Value) return;

        if (Puzzle2Door.AreDoorsOpen()) { enabled = false; return; }

        timeLeft.Value -= Time.deltaTime;

        if (timeLeft.Value <= 0f)
        {
            timeLeft.Value = 0f;
            defeatSent = true;
            ShowDefeatClientRpc();
        }
    }

    [ClientRpc]
    void ShowDefeatClientRpc() =>
        Level2UIManager.Instance?.ShowLosePanel();

    /* Llamado por la Intro (en el servidor) */
    public void StartCountdown()
    {
        if (!countdownStarted.Value)
        {
            countdownStarted.Value = true;
            Debug.Log("⏱️ [SERVER] Cronómetro arrancado.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartLevelServerRpc()
    {
        Puzzle2Manager.Instance?.DespawnAllItemsServerRpc();
        var sceneName = SceneManager.GetActiveScene().name;
        NetworkManager.Singleton.SceneManager
                       .LoadScene(sceneName, LoadSceneMode.Single);
    }
}








