using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PuzzleManager : NetworkBehaviour
{
    public static PuzzleManager Instance;

    [Header("Prefabs y zonas")]
    [SerializeField] private GameObject puzzleItemPrefab;
    [SerializeField] private Collider[] puzzleItemSpawnZones;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupClip;   // sonido de recoger
    [SerializeField] private float pickupVolume = 1f;

    /* ▼ Datos por jugador */
    private readonly Dictionary<ulong, bool> collectedByClient = new();
    private readonly Dictionary<ulong, NetworkObject> itemByClient = new();

    /* ▼ Variables sincronizadas */
    private readonly NetworkVariable<int> collectedCount = new(
        0, NetworkVariableReadPermission.Everyone,
           NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<int> totalRequired = new(
        0, NetworkVariableReadPermission.Everyone,
           NetworkVariableWritePermission.Server);

    private void Awake() => Instance = this;

    /* ─────────────────────────── Ciclo de red ─────────────────────────── */
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitializeExistingClients();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        collectedCount.OnValueChanged += (_, v) => PuzzleUIManager.Instance?.SetCollected(v);
        totalRequired.OnValueChanged += (_, v) => PuzzleUIManager.Instance?.SetTotalRequired(v);

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

    /* ───────────────────── Servidor – spawn y limpieza ───────────────────── */
    private void InitializeExistingClients()
    {
        collectedByClient.Clear();
        itemByClient.Clear();
        totalRequired.Value = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            SpawnItemForClient(client.ClientId);
    }

    private void OnClientConnected(ulong cid) => SpawnItemForClient(cid);

    private void OnClientDisconnected(ulong cid)
    {
        if (itemByClient.TryGetValue(cid, out var netObj) && netObj.IsSpawned)
            netObj.Despawn(true);

        itemByClient.Remove(cid);

        if (collectedByClient.Remove(cid))
            totalRequired.Value--;
    }

    private void SpawnItemForClient(ulong cid)
    {
        collectedByClient[cid] = false;

        Vector3 pos = GetRandomSpawnPosition();
        var obj = Instantiate(puzzleItemPrefab, pos, Quaternion.identity)
                     .GetComponent<NetworkObject>();

        obj.Spawn();
        obj.GetComponent<PuzzleItem>().SetAssignedClientId(cid);

        itemByClient[cid] = obj;
        totalRequired.Value++;
    }

    /* ────────────────────────── Lógica de recolección ───────────────────────── */
    [ServerRpc(RequireOwnership = false)]
    public void NotifyCollectedServerRpc(ulong cid)
    {
        if (!collectedByClient.TryGetValue(cid, out bool already) || already) return;

        collectedByClient[cid] = true;
        collectedCount.Value++;

        /* ▶ Reproduce sonido en todos los clientes en la posición del objeto */
        if (itemByClient.TryGetValue(cid, out NetworkObject obj) && obj != null)
            PlayPickupSoundClientRpc(obj.transform.position);

        if (AllCollected())
            PuzzleDoor.SetAllDoorsOpen();
    }

    /* Sonido de recogida: se ejecuta en TODOS los clientes */
    [ClientRpc]
    private void PlayPickupSoundClientRpc(Vector3 worldPos)
    {
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, worldPos, pickupVolume);
    }

    private bool AllCollected()
    {
        foreach (bool done in collectedByClient.Values)
            if (!done) return false;
        return true;
    }

    /* ───────────────────────────── Utilidades ───────────────────────────── */
    private Vector3 GetRandomSpawnPosition()
    {
        var zone = puzzleItemSpawnZones[Random.Range(0, puzzleItemSpawnZones.Length)];
        var b = zone.bounds;
        return new Vector3(Random.Range(b.min.x, b.max.x), b.min.y, Random.Range(b.min.z, b.max.z));
    }
}








