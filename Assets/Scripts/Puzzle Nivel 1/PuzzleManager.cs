using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : NetworkBehaviour
{
    public static PuzzleManager Instance;

    [SerializeField] private GameObject puzzleItemPrefab;
    [SerializeField] private Collider[] puzzleItemSpawnZones;

    private Dictionary<ulong, bool> collectedByClient = new Dictionary<ulong, bool>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            ulong clientId = client.ClientId;
            collectedByClient[clientId] = false;

            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject item = Instantiate(puzzleItemPrefab, spawnPos, Quaternion.identity);
            item.GetComponent<NetworkObject>().Spawn();

            var itemScript = item.GetComponent<PuzzleItem>();
            itemScript.SetAssignedClientId(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyCollectedServerRpc(ulong clientId)
    {
        collectedByClient[clientId] = true;
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
}



