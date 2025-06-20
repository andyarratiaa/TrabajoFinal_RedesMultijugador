using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PuzzleDoor : NetworkBehaviour
{
    private static List<PuzzleDoor> allDoors = new List<PuzzleDoor>();

    private Animator animator;

    [SerializeField] private NetworkVariable<bool> puertaAbierta = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        allDoors.Add(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        allDoors.Remove(this);
    }

    public override void OnNetworkSpawn()
    {
        puertaAbierta.OnValueChanged += (oldValue, newValue) =>
        {
            if (newValue)
            {
                animator.SetBool("Abierto", true); // Ejecutar animación
            }
        };

        // Si ya estaba abierto al llegar a la escena
        if (puertaAbierta.Value)
        {
            animator.SetBool("Abierto", true);
        }
    }

    // Llamado por el PuzzleManager cuando todos los jugadores han recogido su objeto
    public static void SetAllDoorsOpen()
    {
        foreach (var door in allDoors)
        {
            if (door.IsServer)
            {
                door.puertaAbierta.Value = true; // Esto sincroniza el valor a todos los clientes
            }
        }
    }

    public static bool AreDoorsOpen()
    {
        foreach (var door in allDoors)
        {
            if (!door.puertaAbierta.Value)
                return false;
        }
        return true;
    }
}







