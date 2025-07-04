// Assets/Scripts/Puzzle Nivel 2/Puzzle2Manager.cs
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Puzzle2Manager : NetworkBehaviour
{
    public static Puzzle2Manager Instance;

    [Header("Prefabs (Item A / Item B)")]
    [SerializeField] private GameObject itemPrefabA;
    [SerializeField] private GameObject itemPrefabB;
    [SerializeField] private Collider[] itemSpawnZones;

    /* ─── NUEVO: sonido al recoger ─── */
    [Header("Audio")]
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private float pickupVolume = 1f;

    // ▼ Diccionarios por jugador
    private readonly Dictionary<ulong, bool> collectedAByClient = new();
    private readonly Dictionary<ulong, bool> collectedBByClient = new();
    private readonly Dictionary<ulong, NetworkObject> itemAByClient = new();
    private readonly Dictionary<ulong, NetworkObject> itemBByClient = new();

    // ▼ Variables sincronizadas
    private readonly NetworkVariable<int> collectedA = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<int> collectedB = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<int> totalRequiredA = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<int> totalRequiredB = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

        // Hooks UI
        collectedA.OnValueChanged += (_, v) => Puzzle2UIManager.Instance?.SetCollectedA(v);
        totalRequiredA.OnValueChanged += (_, v) => Puzzle2UIManager.Instance?.SetTotalRequiredA(v);
        collectedB.OnValueChanged += (_, v) => Puzzle2UIManager.Instance?.SetCollectedB(v);
        totalRequiredB.OnValueChanged += (_, v) => Puzzle2UIManager.Instance?.SetTotalRequiredB(v);

        // Estado inicial
        Puzzle2UIManager.Instance?.SetCollectedA(collectedA.Value);
        Puzzle2UIManager.Instance?.SetTotalRequiredA(totalRequiredA.Value);
        Puzzle2UIManager.Instance?.SetCollectedB(collectedB.Value);
        Puzzle2UIManager.Instance?.SetTotalRequiredB(totalRequiredB.Value);
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
        collectedAByClient.Clear();
        collectedBByClient.Clear();
        itemAByClient.Clear();
        itemBByClient.Clear();

        totalRequiredA.Value = 0;
        totalRequiredB.Value = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            SpawnItemsForClient(client.ClientId);
    }

    private void OnClientConnected(ulong clientId) => SpawnItemsForClient(clientId);

    private void OnClientDisconnected(ulong clientId)
    {
        DespawnIfExists(itemAByClient, clientId);
        DespawnIfExists(itemBByClient, clientId);

        if (collectedAByClient.Remove(clientId)) totalRequiredA.Value--;
        if (collectedBByClient.Remove(clientId)) totalRequiredB.Value--;
    }

    private void SpawnItemsForClient(ulong clientId)
    {
        /* Item A */
        collectedAByClient[clientId] = false;
        var objA = Instantiate(itemPrefabA, GetRandomSpawnPosition(), Quaternion.identity)
                       .GetComponent<NetworkObject>();
        objA.Spawn();
        objA.GetComponent<Puzzle2Item>().Initialize(clientId, Puzzle2ItemKind.ItemA);
        itemAByClient[clientId] = objA;
        totalRequiredA.Value++;

        /* Item B */
        collectedBByClient[clientId] = false;
        var objB = Instantiate(itemPrefabB, GetRandomSpawnPosition(), Quaternion.identity)
                       .GetComponent<NetworkObject>();
        objB.Spawn();
        objB.GetComponent<Puzzle2Item>().Initialize(clientId, Puzzle2ItemKind.ItemB);
        itemBByClient[clientId] = objB;
        totalRequiredB.Value++;
    }

    private void DespawnIfExists(Dictionary<ulong, NetworkObject> dict, ulong clientId)
    {
        if (dict.TryGetValue(clientId, out var netObj))
        {
            if (netObj != null && netObj.IsSpawned)
                netObj.Despawn(true);
            dict.Remove(clientId);
        }
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Servidor – notificación de recolección
    // ──────────────────────────────────────────────────────────────────────────────
    [ServerRpc(RequireOwnership = false)]
    public void NotifyCollectedServerRpc(ulong clientId, int kindInt)
    {
        var kind = (Puzzle2ItemKind)kindInt;
        NetworkObject pickedObj = null;                         // NUEVO

        switch (kind)
        {
            case Puzzle2ItemKind.ItemA:
                if (!collectedAByClient.TryGetValue(clientId, out bool doneA) || doneA) return;
                collectedAByClient[clientId] = true;
                collectedA.Value++;
                itemAByClient.TryGetValue(clientId, out pickedObj);   // NUEVO
                break;

            case Puzzle2ItemKind.ItemB:
                if (!collectedBByClient.TryGetValue(clientId, out bool doneB) || doneB) return;
                collectedBByClient[clientId] = true;
                collectedB.Value++;
                itemBByClient.TryGetValue(clientId, out pickedObj);   // NUEVO
                break;
        }

        /* NUEVO: sonido en todos los clientes */
        if (pickedObj != null)
            PlayPickupSoundClientRpc(pickedObj.transform.position);

        if (AllCollected())
            CoopSwitchManager.Instance?.NotifySwitchChanged();
    }

    /* NUEVO: RPC que reproduce el sonido localmente */
    [ClientRpc]
    private void PlayPickupSoundClientRpc(Vector3 worldPos)
    {
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, worldPos, pickupVolume);
    }

    private bool AllCollected() =>
        collectedA.Value >= totalRequiredA.Value &&
        collectedB.Value >= totalRequiredB.Value;

    public bool AllObjectsCollected => AllCollected();
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Utilidades
    // ──────────────────────────────────────────────────────────────────────────────
    private Vector3 GetRandomSpawnPosition()
    {
        var zone = itemSpawnZones[Random.Range(0, itemSpawnZones.Length)];
        var b = zone.bounds;
        return new Vector3(Random.Range(b.min.x, b.max.x), b.min.y, Random.Range(b.min.z, b.max.z));
    }
    #endregion

    [ServerRpc(RequireOwnership = false)]
    public void DespawnAllItemsServerRpc()
    {
        foreach (var n in itemAByClient.Values) if (n != null && n.IsSpawned) n.Despawn(true);
        foreach (var n in itemBByClient.Values) if (n != null && n.IsSpawned) n.Despawn(true);
    }
}
