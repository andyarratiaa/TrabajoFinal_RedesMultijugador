using Unity.Netcode;
using UnityEngine;

public class PuzzleItem : NetworkBehaviour
{
    private NetworkVariable<ulong> assignedClientId = new NetworkVariable<ulong>();
    private bool isCollected = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        if (NetworkManager.Singleton.LocalClientId == assignedClientId.Value && Input.GetKeyDown(KeyCode.E))
        {
            isCollected = true;
            Collect();
        }
    }

    private void Collect()
    {
        TryDespawnServerRpc(); // Solo notificamos al servidor
    }
   
    [ServerRpc(RequireOwnership = false)]
    private void TryDespawnServerRpc(ServerRpcParams rpcParams = default)
    {
        PuzzleManager.Instance?.NotifyCollectedServerRpc(assignedClientId.Value);
        GetComponent<NetworkObject>().Despawn(); // Esto sí lo elimina en todos los clientes sincronizados
    }



    public void SetAssignedClientId(ulong clientId)
    {
        if (IsServer)
        {
            assignedClientId.Value = clientId;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyCollectedServerRpc(ulong clientId)
    {
        PuzzleManager.Instance?.NotifyCollectedServerRpc(clientId);
    }
}





