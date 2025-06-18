using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject playerToSpawn; // Need to be on the network list and loaded in the network manager

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(CallSpawnWithDelay());
    }

    IEnumerator CallSpawnWithDelay()
    {
        yield return new WaitForSeconds(0.3f);
        SpawnLocalPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnLocalPlayerServerRpc(ulong ownerID)
    {
        NetworkObject playerSpawned = Instantiate(playerToSpawn);
        playerSpawned.SpawnWithOwnership(ownerID, true);
    }
}

