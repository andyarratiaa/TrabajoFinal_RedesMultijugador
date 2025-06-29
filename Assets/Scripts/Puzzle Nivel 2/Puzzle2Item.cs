// Assets/Scripts/Puzzle Nivel 2/Puzzle2Item.cs
using Unity.Netcode;
using UnityEngine;

/* El enum sigue igual */
public enum Puzzle2ItemKind { ItemA, ItemB }

public class Puzzle2Item : NetworkBehaviour
{
    /*  ►► 1. Variables sincronizadas  ◄◄  */
    private readonly NetworkVariable<ulong> assignedClientId = new();
    private readonly NetworkVariable<int> kindValue = new();   // 0 = A, 1 = B

    private bool isCollected = false;

    /* Propiedad de conveniencia */
    public Puzzle2ItemKind Kind => (Puzzle2ItemKind)kindValue.Value;

    /*  ►► 2. Inicializa SOLO en el servidor  ◄◄  */
    public void Initialize(ulong clientId, Puzzle2ItemKind kind)
    {
        if (!IsServer) return;

        assignedClientId.Value = clientId;
        kindValue.Value = (int)kind;   // se replica automáticamente
    }

    /*  ►► 3. Recolección (ejecuta en cada cliente)  ◄◄  */
    private void OnTriggerStay(Collider other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        /* Solo el dueño de este objeto puede recogerlo */
        if (NetworkManager.Singleton.LocalClientId == assignedClientId.Value &&
            Input.GetKeyDown(KeyCode.E))
        {
            isCollected = true;
            Collect();
        }
    }

    /*  ►► 4. Notifica al servidor y despawnea  ◄◄  */
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
