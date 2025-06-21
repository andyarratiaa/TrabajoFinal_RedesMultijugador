using Unity.Netcode;
using UnityEngine;

public enum Puzzle2ItemKind { ItemA, ItemB }

public class Puzzle2Item : NetworkBehaviour
{
    private NetworkVariable<ulong> assignedClientId = new();
    [SerializeField] public Puzzle2ItemKind Kind;

    private bool isCollected = false;

    public void Initialize(ulong clientId, Puzzle2ItemKind kind)
    {
        if (IsServer)
        {
            assignedClientId.Value = clientId;
            Kind = kind;
        }
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
        TryNotifyAndDespawnServerRpc((int)Kind, assignedClientId.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryNotifyAndDespawnServerRpc(int kindInt, ulong clientId, ServerRpcParams rpcParams = default)
    {
        Puzzle2Manager.Instance?.NotifyCollectedServerRpc(clientId, kindInt);
        GetComponent<NetworkObject>().Despawn();
    }
}
