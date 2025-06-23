// Assets/Scripts/Coop/CoopSwitchManager.cs
using Unity.Netcode;
using UnityEngine;

public class CoopSwitchManager : NetworkBehaviour
{
    public static CoopSwitchManager Instance { get; private set; }

    [Header("Referencia a los DOS botones")]
    [SerializeField] CoopSwitch leftSwitch;
    [SerializeField] CoopSwitch rightSwitch;

    [Header("Tiempo que deben mantenerse (s)")]
    [SerializeField] float holdTime = 1f;

    private float timer;

    private void Awake() => Instance = this;

    /* ---------- Llamado por cada botón al cambiar ---------- */
    public void NotifySwitchChanged()
    {
        if (!IsServer) return;

        bool ready = leftSwitch.IsPressed.Value && rightSwitch.IsPressed.Value;
        bool collected = Puzzle2Manager.Instance != null &&
                         Puzzle2Manager.Instance.AllObjectsCollected;

        timer = ready && collected ? holdTime : 0f;
    }

    private void Update()
    {
        if (!IsServer || timer <= 0f) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Puzzle2Door.SetAllDoorsOpen();
            enabled = false;              // no hace falta seguir comprobando
        }
    }
}


