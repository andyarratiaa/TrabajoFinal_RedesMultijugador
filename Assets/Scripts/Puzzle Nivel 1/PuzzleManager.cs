using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PuzzleManager : NetworkBehaviour
{
    public static PuzzleManager Instance;

    [Header("Set in Inspector")]
    [SerializeField] private GameObject puzzleItemPrefab;
    [SerializeField] private Collider[] puzzleItemSpawnZones;

    // ▼ Datos por jugador
    private readonly Dictionary<ulong, bool> collectedByClient = new();
    private readonly Dictionary<ulong, NetworkObject> itemByClient = new();   // ← NUEVO

    // ▼ Variables sincronizadas
    private readonly NetworkVariable<int> collectedCount = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<int> totalRequired = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake() => Instance = this;

    // ──────────────────────────────────────────────────────────────────────────────
    #region Ciclo de red
    // ──────────────────────────────────────────────────────────────────────────────
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitializeExistingClients();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        collectedCount.OnValueChanged += (_, newVal) =>
            PuzzleUIManager.Instance?.SetCollected(newVal);

        totalRequired.OnValueChanged += (_, newVal) =>
            PuzzleUIManager.Instance?.SetTotalRequired(newVal);

        PuzzleUIManager.Instance?.SetCollected(collectedCount.Value);
        PuzzleUIManager.Instance?.SetTotalRequired(totalRequired.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Servidor – spawn y limpieza por jugador
    // ──────────────────────────────────────────────────────────────────────────────
    private void InitializeExistingClients()
    {
        collectedByClient.Clear();
        itemByClient.Clear();
        totalRequired.Value = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            SpawnItemForClient(client.ClientId);
    }

    private void OnClientConnected(ulong clientId) => SpawnItemForClient(clientId);

    private void OnClientDisconnected(ulong clientId)
    {
        // 1) Despawn y destroy del objeto del jugador que se va
        if (itemByClient.TryGetValue(clientId, out var netObj))
        {
            if (netObj != null && netObj.IsSpawned)
                netObj.Despawn(true);      // true ⇒ también Destroy()
            itemByClient.Remove(clientId);
        }

        // 2) Ajustar las estructuras lógicas
        if (collectedByClient.Remove(clientId))
            totalRequired.Value--;
    }

    private void SpawnItemForClient(ulong clientId)
    {
        collectedByClient[clientId] = false;

        Vector3 pos = GetRandomSpawnPosition();
        var itemObj = Instantiate(puzzleItemPrefab, pos, Quaternion.identity)
                          .GetComponent<NetworkObject>();

        itemObj.Spawn();
        itemObj.GetComponent<PuzzleItem>().SetAssignedClientId(clientId);

        // Guardar referencia para poder despawnearlo luego
        itemByClient[clientId] = itemObj;

        totalRequired.Value++;
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Lógica de recolección
    // ──────────────────────────────────────────────────────────────────────────────
    [ServerRpc(RequireOwnership = false)]
    public void NotifyCollectedServerRpc(ulong clientId)
    {
        if (!collectedByClient.TryGetValue(clientId, out bool already) || already) return;

        collectedByClient[clientId] = true;
        collectedCount.Value++;

        if (AllCollected())
            PuzzleDoor.SetAllDoorsOpen();
    }

    private bool AllCollected()
    {
        foreach (bool done in collectedByClient.Values)
            if (!done) return false;
        return true;
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Utilidades privadas
    // ──────────────────────────────────────────────────────────────────────────────
    private Vector3 GetRandomSpawnPosition()
    {
        var zone = puzzleItemSpawnZones[Random.Range(0, puzzleItemSpawnZones.Length)];
        var b = zone.bounds;
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.min.y,
            Random.Range(b.min.z, b.max.z)
        );
    }
    #endregion
}







