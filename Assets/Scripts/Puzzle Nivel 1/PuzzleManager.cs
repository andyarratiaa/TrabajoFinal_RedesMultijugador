using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : NetworkBehaviour
{
    public static PuzzleManager Instance;

    [SerializeField] private GameObject puzzleItemPrefab;
    [SerializeField] private Collider[] puzzleItemSpawnZones;

    private Dictionary<ulong, bool> collectedByClient = new Dictionary<ulong, bool>();
    private NetworkVariable<int> collectedCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private int totalRequired = 0;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            totalRequired = 0;
            collectedByClient.Clear();

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ulong clientId = client.ClientId;
                collectedByClient[clientId] = false;

                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject item = Instantiate(puzzleItemPrefab, spawnPos, Quaternion.identity);
                item.GetComponent<NetworkObject>().Spawn();

                var itemScript = item.GetComponent<PuzzleItem>();
                itemScript.SetAssignedClientId(clientId);

                totalRequired++;
            }

            SetTotalRequiredClientRpc(totalRequired);
        }

        collectedCount.OnValueChanged += (oldValue, newValue) =>
        {
            if (PuzzleUIManager.Instance != null)
            {
                PuzzleUIManager.Instance.SetCollected(newValue);
            }
        };

        // Mostrar valores iniciales al entrar a escena
        if (PuzzleUIManager.Instance != null)
        {
            PuzzleUIManager.Instance.SetCollected(collectedCount.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyCollectedServerRpc(ulong clientId)
    {
        if (!collectedByClient.ContainsKey(clientId) || collectedByClient[clientId])
            return;

        collectedByClient[clientId] = true;
        collectedCount.Value++;

        if (AllCollected())
        {
            PuzzleDoor.SetAllDoorsOpen();
        }
    }

    private bool AllCollected()
    {
        foreach (bool collected in collectedByClient.Values)
            if (!collected) return false;
        return true;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Collider zone = puzzleItemSpawnZones[Random.Range(0, puzzleItemSpawnZones.Length)];
        Bounds bounds = zone.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    [ClientRpc]
    private void SetTotalRequiredClientRpc(int total)
    {
        if (PuzzleUIManager.Instance != null)
        {
            PuzzleUIManager.Instance.SetTotalRequired(total);
        }
    }
}





