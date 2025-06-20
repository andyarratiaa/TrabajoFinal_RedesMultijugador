using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PuzzleManager : NetworkBehaviour
{
    public static PuzzleManager Instance;

    [Header("Set in Inspector")]
    [SerializeField] private GameObject puzzleItemPrefab;
    [SerializeField] private Collider[] puzzleItemSpawnZones;

    // ▼ Datos por-jugador
    private readonly Dictionary<ulong, bool> collectedByClient = new();

    // ▼ Variables sincronizadas
    private readonly NetworkVariable<int> collectedCount = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<int> totalRequired = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake() => Instance = this;

    // ──────────────────────────────────────────────────────────────────────────────
    #region Network lifecycle
    // ──────────────────────────────────────────────────────────────────────────────
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // 1) Objetos para los clientes que YA están
            InitializeExistingClients();

            // 2) Objetos para los que lleguen después
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        // Actualizar HUD cuando cambie algo
        collectedCount.OnValueChanged += (_, newValue) =>
            PuzzleUIManager.Instance?.SetCollected(newValue);

        totalRequired.OnValueChanged += (_, newValue) =>
            PuzzleUIManager.Instance?.SetTotalRequired(newValue);

        // Mostrar valores actuales a quien acaba de entrar
        PuzzleUIManager.Instance?.SetCollected(collectedCount.Value);
        PuzzleUIManager.Instance?.SetTotalRequired(totalRequired.Value);
    }

    private void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Servidor – spawn por jugador
    // ──────────────────────────────────────────────────────────────────────────────
    private void InitializeExistingClients()
    {
        collectedByClient.Clear();
        totalRequired.Value = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            SpawnItemForClient(client.ClientId);
    }

    private void OnClientConnected(ulong clientId) => SpawnItemForClient(clientId);

    private void OnClientDisconnected(ulong clientId)
    {
        // Limpieza opcional: ajustar el total para que la puerta se siga abriendo
        if (collectedByClient.Remove(clientId))
            totalRequired.Value--;
    }

    private void SpawnItemForClient(ulong clientId)
    {
        collectedByClient[clientId] = false;

        Vector3 pos = GetRandomSpawnPosition();
        var itemObj = Instantiate(puzzleItemPrefab, pos, Quaternion.identity);

        itemObj.GetComponent<NetworkObject>().Spawn();
        itemObj.GetComponent<PuzzleItem>().SetAssignedClientId(clientId);

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






