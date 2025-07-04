using Unity.Netcode;
using UnityEngine;

public enum Puzzle2ItemKind { ItemA, ItemB }

public class Puzzle2Item : NetworkBehaviour
{
    private readonly NetworkVariable<ulong> assignedClientId = new();
    private readonly NetworkVariable<int> kindValue = new();

    private bool isCollected = false;

    public Puzzle2ItemKind Kind => (Puzzle2ItemKind)kindValue.Value;

    public void Initialize(ulong clientId, Puzzle2ItemKind kind)
    {
        if (!IsServer) return;

        assignedClientId.Value = clientId;
        kindValue.Value = (int)kind;  
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        if (NetworkManager.Singleton.LocalClientId == assignedClientId.Value &&
            Input.GetKeyDown(KeyCode.E))
        {
            isCollected = true;
            Collect();
        }
    }

    private void Collect()
    {
        TryNotifyAndDespawnServerRpc(kindValue.Value, assignedClientId.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryNotifyAndDespawnServerRpc(int kindInt, ulong clientId,
                                              ServerRpcParams _ = default)
    {
        Puzzle2Manager.Instance?.NotifyCollectedServerRpc(clientId, kindInt);
        GetComponent<NetworkObject>().Despawn();
    }
}
